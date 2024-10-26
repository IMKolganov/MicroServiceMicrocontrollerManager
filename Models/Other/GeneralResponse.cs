namespace MicroServiceMicrocontrollerManager.Models.Other;

public class GeneralResponse<T>
{
    public string RequestId { get; set; }
    public string ResponseType { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public T Data { get; set; }
}