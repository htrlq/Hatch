using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggerUtil;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication1
{
    public class ContextA : SeriLogger
    {
        public ContextA(SeriLoggerConfiguraBuild configuraBuild, TableBuild build, LoggerBuild loggerBuild) : base(configuraBuild, build, loggerBuild)
        {
        }
    }

    public class ContextB : SeriLogger
    {
        public ContextB(SeriLoggerConfiguraBuild configuraBuild, TableBuild build, LoggerBuild loggerBuild) : base(configuraBuild, build, loggerBuild)
        {
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSeriLogger();

            services
                .AddSeriLoggerContextPool<ContextA>()
                .AddSeriLoggerContextPool<ContextB>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app
                .UseSeriLogger<SeriLogger>(obj =>
                {
                    obj.Seq = "http://127.0.0.1:5341";
                    //obj.ConnectString = "server=127.0.0.1,1401;database=Serilog;uid=sa;pwd=hTrlq20181123!;";
                })
                .UseSeriLogger<ContextA>(obj =>
                {
                    obj.Seq = new SeqConfiguration()
                    {
                        Url = "http://127.0.0.1:5341",
                        ApiKey = "sacPmoyNrEqQTONHagvd"
                    };
                })
                .UseSeriLogger<ContextB>(obj =>
                {
                    obj.Seq = new SeqConfiguration()
                    {
                        Url = "http://127.0.0.1:5341",
                        ApiKey = "z6FhLBHDHuYkTWA9eeDE"
                    };
                });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
