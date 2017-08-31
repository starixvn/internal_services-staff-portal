namespace SSP.Services
{
    using SSP.Core.Services;
    using SSP.Criteria.Function;
    using SSP.Models.Function;
    using System.Collections.Generic;

    public class FunctionService : BaseService, IFunctionService
    {
        public IEnumerable<Function> GetFunctions()
        {
            var criteria = new GetFunctionCriteria();
            IEnumerable<Function> functions = GetData<Function>(criteria);

            return functions;
        }
    }
}