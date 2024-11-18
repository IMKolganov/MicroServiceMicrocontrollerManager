using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;
using MicroServiceMicrocontrollerManager.Models.Other;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace MicroServiceMicrocontrollerManager.Services;


public class MqttService
{
    private readonly IMqttClient _mqttClient;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public MqttService(IOptions<MqttSettings> mqttSettings)
    {
        var settings = mqttSettings.Value;
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(settings.ClientId)
            .WithTcpServer(settings.Host, settings.Port)
            .Build();

        Task.Run(async () => await _mqttClient.ConnectAsync(options)).Wait();
    }

    public async Task<TResponse?> SendRequestAndWaitForResponse<TRequest, TResponse>(string requestTopic,
        string responseTopic, TRequest request)
        where TRequest : IRequest
        where TResponse : class
    {
        var responseReceived = new TaskCompletionSource<TResponse?>();

        Console.WriteLine($"Subscribing to response topic: {responseTopic}");
        await _mqttClient.SubscribeAsync(responseTopic);

        Func<MqttApplicationMessageReceivedEventArgs, Task> messageHandler = async e =>
        {
            await Task.Yield();

            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.ToArray());
            Console.WriteLine($"Received message on response topic: {message}");

            try
            {
                var response = JsonConvert.DeserializeObject<TResponse>(message);

                var requestIdProperty = typeof(TResponse).GetProperty("RequestId");
                var responseRequestId = requestIdProperty?.GetValue(response)?.ToString();

                if (responseRequestId == request.RequestId.ToString())
                {
                    responseReceived.TrySetResult(response);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize message: {ex.Message}");
            }
        };

        _mqttClient.ApplicationMessageReceivedAsync += messageHandler;

        try
        {
            var payload = JsonConvert.SerializeObject(request);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(requestTopic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            Console.WriteLine($"Publishing message to request topic: {requestTopic}");
            await _mqttClient.PublishAsync(message);

            var result = await Task.WhenAny(responseReceived.Task, Task.Delay(5000)) == responseReceived.Task
                ? responseReceived.Task.Result
                : default;

            if (result == null)
            {
                Console.WriteLine("MicrocontrollerManager: No response from MQTT within timeout.");
                throw new Exception("MicrocontrollerManager: No response from MQTT within timeout.");
            }

            return result;
        }
        finally
        {
            await _mqttClient.UnsubscribeAsync(responseTopic);
            _mqttClient.ApplicationMessageReceivedAsync -= messageHandler;
        }
    }


}