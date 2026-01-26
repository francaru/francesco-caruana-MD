namespace Messaging;

public sealed class MQEventInfo
{
    public required string QueueName {  get; init; }
    
    public required MQEventRecipient Sender { get; init; }
}
