using System;
using LoggerUtil;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerilogController : ControllerBase
    {
        private ISeriLogger Log { get; }
        private ContextA ContextA { get; }
        private ContextB ContextB { get; }

        public SerilogController(ISeriLogger log, ContextA contextA, ContextB contextB)
        {
            Log = log;
            ContextA = contextA;
            ContextB = contextB;
        }

        // GET api/values
        [HttpGet]
        public string Write()
        {
            var model = new UserModel()
            {
                User = "admin",
                Account = "system"
            };

            Log.Write(LogEventLevel.Debug, "", model);
            ContextA.Write(LogEventLevel.Debug, "", model);
            ContextB.Write(LogEventLevel.Debug, "", model);

            return "Write";
        }

        private Exception Excepts =
            new Exception("node", new Exception("Node Parent", new Exception("Node Parent Parent")));

        [HttpGet("Error")]
        public string Error()
        {
            Log.Error(Excepts);
            return "Error";
        }

        [HttpGet("Fatal")]
        public string Fatal()
        {
            Log.Fatal(Excepts);
            return "Fatal";
        }

        [HttpGet("Debug")]
        public string Debug()
        {
            Log.Debug(Excepts);
            return "Debug";
        }

        [HttpGet("Information")]
        public string Information()
        {
            Log.Information(Excepts);
            return "Information";
        }

        [ResponseException]
        [Logger]
        [HttpGet("TestException")]
        public void TestException()
        {
            throw Excepts;
        }
    }

    public class UserModel
    {
        public string User { get; set; }
        public string Account { get; set; }
    }
}
