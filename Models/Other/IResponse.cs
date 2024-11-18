namespace MicroServiceMicrocontrollerManager.Models.Other;

public interface IResponse
{
    Guid RequestId { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
    public DateTime CreateDate { get; set; }
}