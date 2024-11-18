using Newtonsoft.Json.Linq;

namespace MicroServiceMicrocontrollerManager.Models.Other;

public class GeneralRequest
{
    public Guid RequestId { get; set; }
    public string RequestType { get; set; }
    public object Data { get; set; }
}