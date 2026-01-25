namespace Messaging.LifecycleEvents;

public sealed record StatusChangeEventBody : MQEventBody
{
    public string PreviousStatus { get; init; }

    public string NewStatus { get; init; }

    public StatusChangeEventBody(string previousStatus, string newStatus)
    {
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}
