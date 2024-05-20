using ExtService.GateWay.API.Abstractions.Strategy;

namespace ExtService.GateWay.API.Abstractions.Factories
{
    public interface IBillingStrategyFactory
    {
        IBillingStrategy GetBillingStrategy();
    }
}
