using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using connectingToDBTESTING.Models;
using MySQL.Data.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace connectingToDBTESTING
{
    public class Startup
    {
        // public IConfiguration Configuration { get; private set;}
        // // This method gets called by the runtime. Use this method to add services to the container.
        // public Startup(IHostingEnvironment env)
        // {
        //     var builder = new ConfigurationBuilder()
        //     .SetBasePath(env.ContentRootPath)
        //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange:true)
        //     .AddEnvironmentVariables();
        //     Configuration = builder.Build();
        // }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ActivitiesContext>(options => options.UseMySQL(Configuration["DBInfo:ConnectionString"]));
            //services.Configure<MySqlOptions>(Configuration.GetSection("DBInfo"));
            //services.AddScoped<DbConnector>();
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc();
        }
        public IConfiguration Configuration { get; private set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
    }
}
