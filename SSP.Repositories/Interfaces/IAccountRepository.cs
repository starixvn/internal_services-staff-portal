namespace SSP.Repositories
{
    using Fanex.Data.Repository;
    using SSP.Models.Account;
    using System.Threading.Tasks;

    public interface IAccountRepository
    {
        Task<UserSession> GetUserLoginAsync(ICriteria criteria);
    }
}