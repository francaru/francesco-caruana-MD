namespace Messaging;

public sealed record MQEventRecipient
{
    public string ServiceName { get; init; }

    public MQEventRecipient(string serviceName)
    {
        ServiceName = serviceName;
    }
}
