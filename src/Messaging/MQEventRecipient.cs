namespace Messaging;

public sealed record MQEventRecipient
{
    public required string ServiceName { get; init; }
}
