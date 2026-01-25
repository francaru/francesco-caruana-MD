namespace Messaging;

public sealed record MQEvent<T> where T : MQEventBody 
{
    public T Body { get; init; }

    public MQEventRecipient Recipient { get; init; }

    public MQEvent(T body, MQEventRecipient recipient)
    {
        Body = body;
        Recipient = recipient;
    }
}
