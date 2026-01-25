namespace Messaging.LifecycleEvents;

public sealed record ProgressChangeEventBody : MQEventBody
{
    public string CurrentStatus { get; init; }

    public double ProgressPercent { get; init; }

    public ProgressChangeEventBody(string currentStatus, double progressPercent)
    {
        CurrentStatus = currentStatus;
        ProgressPercent = progressPercent;
    }
}
