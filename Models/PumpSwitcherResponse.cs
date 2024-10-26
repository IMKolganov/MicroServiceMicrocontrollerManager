using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class PumpSwitcherResponse(string requestId, int pumpId, bool success, string message)
    : IResponse
{
    public string RequestId { get; set; } = requestId;
    public int PumpId { get; set; } = pumpId;
    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
}