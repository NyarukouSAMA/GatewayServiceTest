﻿using ExtService.GateWay.API.Abstractions.Services;

namespace ExtService.GateWay.API.Abstractions.Factories
{
    public interface ILimitCheckServiceFactory
    {
        ILimitCheckService GetLimitCheckService();
    }
}
