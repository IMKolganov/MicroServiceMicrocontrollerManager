using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class PumpSwitcherRequest(string requestId) : IRequest
{
    public string RequestId { get; set; } = requestId;
    public int PumpId { get; set; }
    public int Seconds { get; set; }
}