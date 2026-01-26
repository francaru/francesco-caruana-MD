namespace Messaging;

public sealed record MQEvent<T> where T : MQEventBody 
{
    public required T Body { get; init; }

    public required MQEventRecipient Recipient { get; init; }
}
