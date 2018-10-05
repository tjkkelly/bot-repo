using Microsoft.EntityFrameworkCore.Design;
using TheCountBot.Data;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace TheCountBot.Factories
{
    public class NumberStoreContextFactory : IDesignTimeDbContextFactory<NumberStoreContext>
    {
        public NumberStoreContext CreateDbContext( string[] args )
        {
            IConfigurationRoot configuration = ConfigurationRootFactory.CreateConfigurationBuilder();

            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.Configure<Settings>( configuration );
            Settings settings = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<Settings>>().Value;

            serviceCollection.AddDbContext<NumberStoreContext>( options => options.UseSqlServer( settings.SqlConnectionStringReadWrite ) );

            return serviceCollection.BuildServiceProvider().GetRequiredService<NumberStoreContext>();
        }
    }
}