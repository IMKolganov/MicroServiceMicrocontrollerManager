namespace MicroServiceMicrocontrollerManager.Models.Other;

public interface IRequest
{
    Guid RequestId { get; set; }
    bool UseRandomValuesFotTest { get; set; }
    DateTime RequestDate { get; set; }
}