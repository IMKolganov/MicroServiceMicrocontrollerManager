using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class TemperatureHumidityResponse(
    Guid requestId,
    bool success,
    string message,
    int sensorId,
    int temperature,
    int humidity)
    : IResponse
{
    public Guid RequestId { get; set; } = requestId;
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public int SensorId { get; set; } = sensorId;
    public int Temperature { get; set; } = temperature;
    public int Humidity { get; set; } = humidity;
}