namespace SSP.Core.Services
{
    using Fanex.Data.Repository;
    using Repositories;
    using System.Collections.Generic;

    public class BaseService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository { get; set; }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }
    }
}