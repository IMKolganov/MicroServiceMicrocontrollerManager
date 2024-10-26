namespace MicroServiceMicrocontrollerManager.Models.Other;

public class MqttSettings
{
    public string? Host { get; init; }
    public int Port { get; init; }
    public string? ClientId { get; init; }

    public MqttSettings() { }
}
