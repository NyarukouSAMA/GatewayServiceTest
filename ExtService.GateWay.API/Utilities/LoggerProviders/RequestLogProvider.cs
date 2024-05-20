using Serilog.Extensions.Logging;

namespace ExtService.GateWay.API.Utilities.LoggerProviders
{
    public class RequestLogProvider : SerilogLoggerProvider
    {
        public RequestLogProvider(Serilog.ILogger logger) : base(logger)
        {
        }
    }
}
