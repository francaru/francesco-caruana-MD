using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace OperationalService;

public class ActivityEventLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        base.OnEnd(data);
        var currentActivity = Activity.Current;

        if (currentActivity is not null)
        {
            currentActivity.AddEvent(new ActivityEvent(data!.Attributes!.ToString()!));
        }
    }
}
