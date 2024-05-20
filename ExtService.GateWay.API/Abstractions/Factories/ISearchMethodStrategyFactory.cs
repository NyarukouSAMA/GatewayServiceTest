using ExtService.GateWay.API.Abstractions.Strategy;

namespace ExtService.GateWay.API.Abstractions.Factories
{
    public interface ISearchMethodStrategyFactory
    {
        IMethodInfoStrategy GetMethodInfoStrategy();
    }
}
