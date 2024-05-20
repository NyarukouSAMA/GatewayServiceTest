using Serilog.Extensions.Logging;

namespace ExtService.GateWay.API.Utilities.LoggerProviders
{
    public class BaseSerilogProvider : SerilogLoggerProvider
    {
        public BaseSerilogProvider(Serilog.ILogger logger) : base(logger)
        {
        }
    }
}
