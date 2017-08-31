namespace SSP.Core.Services
{
    using Fanex.Data.Repository;
    using Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BaseService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository { get; set; }

        public virtual async Task<IEnumerable<T>> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria);
            }

            return new List<T>();
        }

        public virtual async Task<T> GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria);
            }

            return default(T);
        }
    }
}