﻿using MicroServiceMicrocontrollerManager.Models.Other;

namespace MicroServiceMicrocontrollerManager.Models;

public class TemperatureHumidityRequest : IRequest
{
    public TemperatureHumidityRequest()
    {
    }
    public TemperatureHumidityRequest(Guid requestId)
    {
        RequestId = requestId;
    }
    
    public Guid RequestId { get; set; }
    public bool UseRandomValuesFotTest { get; set; }
    public DateTime RequestDate { get; set; }
    public int SensorId { get; set; }
}