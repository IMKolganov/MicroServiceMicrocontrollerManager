using MicroServiceMicrocontrollerManager.Models;
using MicroServiceMicrocontrollerManager.Models.Other;
using Newtonsoft.Json;

namespace MicroServiceMicrocontrollerManager.Services;

public class ProcessingService(MqttService mqttService, RabbitMqService rabbitMqService)
{
    private async Task<PumpSwitcherResponse> ProcessPumpSwitcherAsync(PumpSwitcherRequest request)
    {
        var response = await mqttService.SendRequestAndWaitForResponse<PumpSwitcherRequest, PumpSwitcherResponse>(
            "control/pump/", "status/pump/", request);

        return new PumpSwitcherResponse(
            requestId: request.RequestId,
            pumpId: request.PumpId,
            success: response?.Success ?? false,
            message: response?.Message ?? string.Empty
        );
    }

    private async Task<GetTemperatureHumidityResponse> ProcessTemperatureHumidityAsync(GetTemperatureHumidityRequest request)
    {
        var response = await mqttService.SendRequestAndWaitForResponse<GetTemperatureHumidityRequest, GetTemperatureHumidityResponse>(
            "control/dht/", "status/dht/", request);

        return new GetTemperatureHumidityResponse(
            requestId: request.RequestId,
            success: response?.Success ?? false,
            message: response?.Message ?? string.Empty,
            sensorId: request.SensorId,
            temperature: response?.Temperature ?? 0,
            humidity: response?.Humidity ?? 0
        );
    }

    private async Task<GetSoilMoistureResponse> ProcessSoilMoistureAsync(GetSoilMoistureRequest request)
    {
        var response = await mqttService.SendRequestAndWaitForResponse<GetSoilMoistureRequest, GetSoilMoistureResponse>(
            "control/soil-moisture/", "status/soil-moisture/", request);

        return new GetSoilMoistureResponse(
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
                    // Проверка на null, чтобы избежать ошибки

                    specificResponse = request.RequestType switch
                    {
                        "PumpSwitcher" => await ProcessPumpSwitcherAsync(
                            JsonConvert.DeserializeObject<PumpSwitcherRequest>(request.Data.ToString())
                            ?? throw new InvalidOperationException("Failed to deserialize PumpSwitcherRequest")),
                        "TemperatureHumidity" => await ProcessTemperatureHumidityAsync(
                            JsonConvert.DeserializeObject<GetTemperatureHumidityRequest>(request.Data.ToString())
                            ?? throw new InvalidOperationException(
                                "Failed to deserialize GetTemperatureHumidityRequest")),
                        "SoilMoisture" => await ProcessSoilMoistureAsync(
                            JsonConvert.DeserializeObject<GetSoilMoistureRequest>(request.Data.ToString())
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
                    ResponseType = request.RequestType,
                    Success = success,
                    ErrorMessage = errorMessage ?? string.Empty,
                    Data = specificResponse ?? throw new InvalidOperationException()
                };
            });
    }
}
