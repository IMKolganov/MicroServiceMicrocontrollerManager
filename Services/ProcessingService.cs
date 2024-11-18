using MicroServiceMicrocontrollerManager.Models;
using MicroServiceMicrocontrollerManager.Models.Other;
using Newtonsoft.Json;
using SharedRequests.SmartGarden.Models.Requests;
using SharedRequests.SmartGarden.Models.Responses;

namespace MicroServiceMicrocontrollerManager.Services;

public class ProcessingService(MqttService mqttService, RabbitMqService rabbitMqService)
{
    private async Task<PumpSwitcherResponse> ProcessPumpSwitcherAsync(PumpSwitcherRequest request)
    {
        var response = await mqttService.SendRequestAndWaitForResponse<PumpSwitcherRequest, PumpSwitcherResponse>(
            "control/pump/", "status/pump/", request);

        return new PumpSwitcherResponse()
        {
            RequestId = request.RequestId,
            PumpId = request.PumpId,
            Success = response?.Success ?? false,
            Message = response?.Message ?? String.Empty
        };
    }

    private async Task<TemperatureHumidityResponse> ProcessTemperatureHumidityAsync(TemperatureHumidityRequest request)
    {
        if (request.UseRandomValuesFotTest)
        {
            var random = new Random();
            return new TemperatureHumidityResponse(
                requestId: request.RequestId,
                success: true,
                message: "Generated random values",
                sensorId: request.SensorId,
                temperature: random.Next(-10, 35),
                humidity: random.Next(0, 100)
            );
        }

        var response = await mqttService.SendRequestAndWaitForResponse<TemperatureHumidityRequest, TemperatureHumidityResponse>(
            "control/dht/", "status/dht/", request);

        return new TemperatureHumidityResponse(
            requestId: request.RequestId,
            success: response?.Success ?? false,
            message: response?.Message ?? string.Empty,
            sensorId: request.SensorId,
            temperature: response?.Temperature ?? 0,
            humidity: response?.Humidity ?? 0
        );
    }

    private async Task<SoilMoistureResponse> ProcessSoilMoistureAsync(SoilMoistureRequest request)
    {
        if (request.UseRandomValuesFotTest)
        {
            var random = new Random();
            return new SoilMoistureResponse(
                requestId: request.RequestId,
                success: true,
                message: "Generated random values",
                sensorId: request.SensorId,
                soilMoistureLevelPercent: random.Next(0, 100)
            );
        }
        
        var response = await mqttService.SendRequestAndWaitForResponse<SoilMoistureRequest, SoilMoistureResponse>(
            "control/soil-moisture/", "status/soil-moisture/", request);

        return new SoilMoistureResponse(
            requestId: request.RequestId,
            success: response?.Success ?? false,
            sensorId: request.SensorId,
            message: response?.Message ?? string.Empty,
            soilMoistureLevelPercent: response?.SoilMoistureLevelPercent
        );
    }

    public void StartProcessing()
    {
        rabbitMqService.StartListening<GeneralRequest, GeneralResponse<object>>(
            requestQueueName: "backend.to.msmicrocontrollermanager.request",
            responseQueueName: "msmicrocontrollermanager.to.backend.response",
            onRequestReceived: async request =>
            {
                object? specificResponse = null;
                bool success = true;
                string? errorMessage = null;

                try
                {
                    specificResponse = request.RequestType switch
                    {
                        "PumpSwitcher" => await ProcessPumpSwitcherAsync(
                            JsonConvert.DeserializeObject<PumpSwitcherRequest>(request.Data.ToString())
                            ?? throw new InvalidOperationException("Failed to deserialize PumpSwitcherRequest")),
                        "TemperatureHumidity" => await ProcessTemperatureHumidityAsync(
                            JsonConvert.DeserializeObject<TemperatureHumidityRequest>(request.Data.ToString())
                            ?? throw new InvalidOperationException(
                                "Failed to deserialize GetTemperatureHumidityRequest")),
                        "SoilMoisture" => await ProcessSoilMoistureAsync(
                            JsonConvert.DeserializeObject<SoilMoistureRequest>(request.Data.ToString())
                            ?? throw new InvalidOperationException("Failed to deserialize GetSoilMoistureRequest")),
                        _ => throw new NotImplementedException("Unknown request type")
                    };
                }
                catch (Exception ex)
                {
                    specificResponse = null;
                    success = false;
                    errorMessage = ex.Message;
                }

                return new GeneralResponse<object>
                {
                    RequestId = request.RequestId,
                    Success = success,
                    ErrorMessage = errorMessage ?? string.Empty,
                    Data = specificResponse ?? string.Empty,
                };
            });
    }
}
