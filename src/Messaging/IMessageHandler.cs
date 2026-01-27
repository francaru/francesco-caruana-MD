using Database;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Messaging
{
    public interface IMessageHandler : IDisposable
    {
        string ServiceName { get; }

        void CreateQueue(string queueName);

        void Produce<T>(string[] toQueues, T mqEventBody) where T : MQEventBody;

        void Consume<T>(string onQueue, params Action<DatabaseContext, ActivitySource, ILoggerProvider, MQEventInfo, T?>[] consumerActions) where T : MQEventBody;
    }
}
