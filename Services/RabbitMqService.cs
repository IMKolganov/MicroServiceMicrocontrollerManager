using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace MicroServiceMicrocontrollerManager.Services;

public class RabbitMqService
{
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory() { HostName = configuration["RabbitMQ:Host"] };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

    }

    public void StartListening<TRequest, TResponse>(string requestQueueName, string responseQueueName, Func<TRequest, Task<TResponse>> onRequestReceived)
        where TRequest : class
        where TResponse : class
    {
        _channel.QueueDeclare(queue: requestQueueName, durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var request = JsonConvert.DeserializeObject<TRequest>(message);
            if (request == null)
            {
                Console.WriteLine("Failed to deserialize request message.");
                return;
            }

            var correlationId = ea.BasicProperties.CorrelationId;

            var response = await onRequestReceived(request);

            var responseProperties = _channel.CreateBasicProperties();
            responseProperties.CorrelationId = correlationId;

            var responseMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            _channel.BasicPublish(exchange: "", routingKey: responseQueueName, basicProperties: responseProperties, body: responseMessage);
        };

        _channel.BasicConsume(queue: requestQueueName, autoAck: true, consumer: consumer);
    }
}