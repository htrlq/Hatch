using LoggerUtil;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ISeriLogger Log { get; }

        public ValuesController(ISeriLogger log)
        {
            Log = log;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            Log.Write(LogEventLevel.Debug, "", new UserModel()
            {
                User = "admin",
                Account = "system"
            });
            return new [] { "value1", "value2" };
        }
    }

    public class UserModel
    {
        public string User { get; set; }
        public string Account { get; set; }
    }
}
