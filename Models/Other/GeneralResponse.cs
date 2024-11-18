namespace MicroServiceMicrocontrollerManager.Models.Other;

public class GeneralResponse<T> : IRabbitMqResponse
{
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public T Data { get; set; }
}