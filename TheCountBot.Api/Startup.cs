using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace TheCountBot.Api
{
    public class Startup
    {
        public Startup( IHostingEnvironment hostingEnvironment )
        {
            string debugFileName = $"cntBotSettings.debug.json";
            string releaseFileName = $"cntBotSettings.release.json";

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath( hostingEnvironment.ContentRootPath )
                .AddJsonFile( releaseFileName, optional: true )
                .AddJsonFile( debugFileName, optional: true );

            Configuration = configurationBuilder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddMvc()
                .SetCompatibilityVersion( CompatibilityVersion.Latest );

            services.AddOptions();
            services.Configure<Settings>( Configuration );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Count Bot Api", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Count Bot Api v1");
            });
        }
    }
}
