using Microsoft.AspNetCore.Mvc;
using OperationalService.Api.Schemas;
using OperationalService.BusinessLogic.Services;
using Database;

namespace OperationalService.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthController(DatabaseContext dbContext) : ControllerBase
{
    [HttpGet]
    [Produces<Health>]
    public IActionResult Index()
    {
        var healthService = new HealthService(dbContext);
        var uptime = healthService.GetUpTime(App.StartTime);

        return Ok(
            new Health()
            {
                UpTime = uptime
            }
        );
    }
}
