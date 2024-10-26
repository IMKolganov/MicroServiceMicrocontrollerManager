using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class GetTemperatureHumidityRequest(string requestId, bool withoutMsMicrocontrollerManager) : IRequest
{
    public string RequestId { get; set; } = requestId;
    public int SensorId { get; set; }
    public bool WithoutMsMicrocontrollerManager { get; set; } = withoutMsMicrocontrollerManager;
    public DateTime CreateDate { get; set; }
}