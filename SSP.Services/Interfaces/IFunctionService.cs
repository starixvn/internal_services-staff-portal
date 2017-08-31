namespace SSP.Services
{
    using SSP.Models.Function;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFunctionService
    {
        Task<IEnumerable<Function>> GetFunctionAsync();
    }
}