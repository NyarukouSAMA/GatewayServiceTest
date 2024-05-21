using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExtService.GateWay.API.Utilities.DBUtils
{
    public static class MigrationManager
    {
        public static void ApplyMigration(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            try
            {
                var dbContext = scopedProvider.GetRequiredService<GateWayContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                Log.Logger.Error(ex, "An error occurred while migrating the database.");
            }
        }
    }
}
