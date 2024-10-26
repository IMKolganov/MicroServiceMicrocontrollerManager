namespace MicroServiceMicrocontrollerManager.Models.Other;

public interface IResponse
{
    string RequestId { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
    public DateTime CreateDate { get; set; }
}