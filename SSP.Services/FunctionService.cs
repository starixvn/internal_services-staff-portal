namespace SSP.Services
{
    using SSP.Core.Services;
    using SSP.Criteria.Function;
    using SSP.Models.Function;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FunctionService : BaseService, IFunctionService
    {
        public async Task<IEnumerable<Function>> GetFunctionAsync()
        {
            var criteria = new GetFunctionCriteria();
            IEnumerable<Function> functions = new List<Function>();

            if (criteria.IsValid())
            {
                functions = await GetDataAsync<Function>(criteria);
            }

            return functions;
        }
    }
}