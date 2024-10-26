using Newtonsoft.Json.Linq;

namespace MicroServiceMicrocontrollerManager.Models.Other;

public class GeneralRequest
{
    public string RequestId { get; set; }
    public string RequestType { get; set; }
    public JObject Data { get; set; }
}