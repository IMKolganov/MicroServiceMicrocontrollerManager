using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class GetSoilMoistureRequest(string requestId) : IRequest
{
    public string RequestId { get; set; } = requestId;
    public int SensorId { get; set; }
}