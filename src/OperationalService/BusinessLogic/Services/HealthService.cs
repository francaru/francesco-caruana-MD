using Database;

namespace OperationalService.BusinessLogic.Services
{
    public class HealthService(DatabaseContext dbContext)
    {
        public TimeSpan GetUpTime(DateTime startTime)
        {
            return DateTime.UtcNow - startTime;
        }
    }
}
