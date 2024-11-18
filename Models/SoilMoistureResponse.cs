using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class SoilMoistureResponse(
    Guid requestId,
    bool success,
    int sensorId,
    string message,
    double? soilMoistureLevelPercent)
    : IResponse
{
    public Guid RequestId { get; set; } = requestId;
    public bool Success { get; set; } = success;
    public int SensorId { get; set; } = sensorId;
    public string Message { get; set; } = message;
    public double? SoilMoistureLevelPercent { get; set; } = soilMoistureLevelPercent;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
}