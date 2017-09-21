namespace TicketDesk.Core.Services
{
    using Repositories;

    public class BaseService : Fanex.NAP.Core.Service.Impl.BaseService
    {
        public BaseService(IBaseRepository repository = null)
        {
            this.Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository { get; set; }
    }
}