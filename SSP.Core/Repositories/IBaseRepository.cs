namespace SSP.Core.Repositories
{
    using Fanex.Data.Repository;
    using Models;
    using System.Threading.Tasks;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQuery(ICriteria criteria);
    }
}