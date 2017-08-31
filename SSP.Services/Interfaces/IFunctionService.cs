namespace SSP.Services
{
    using SSP.Models.Function;
    using System.Collections.Generic;

    public interface IFunctionService
    {
        IEnumerable<Function> GetFunctions();
    }
}