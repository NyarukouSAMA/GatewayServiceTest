using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

//[assembly: DesignTimeServicesReference("ExtService.GateWay.DBContext.GateWayDesignTimeServices")]
namespace ExtService.GateWay.DBContext
{
    public class GateWayDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            // We will try this later - it's interesting, however. we don't have enough time for this.
            //System.Diagnostics.Debugger.Launch();

            //// Remove the existing MigrationsModelDiffer service
            //var serviceDescriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(IMigrationsModelDiffer));

            //if (serviceDescriptor != null)
            //{
            //    serviceCollection.Remove(serviceDescriptor);
            //}

            //// Register the custom MigrationsModelDiffer service
            //serviceCollection.AddSingleton<IMigrationsModelDiffer, GatewayMigrationsModelDiffer>();

            //// Register the custom MigrationSqlGenerator service
            //serviceCollection.AddSingleton<ICSharpMigrationOperationGenerator, GateWayCSharpMigrationOperationGenerator>();
        }
    }
}
