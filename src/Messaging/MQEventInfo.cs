namespace Messaging;

public sealed class MQEventInfo
{
    public string QueueName {  get; init; }
    
    public MQEventRecipient Sender { get; init; }

    public MQEventInfo(string queueName, MQEventRecipient sender)
    {
        QueueName = queueName;
        Sender = sender;
    }
}
