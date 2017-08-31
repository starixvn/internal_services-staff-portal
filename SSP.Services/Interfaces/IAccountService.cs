namespace SSP.Services
{
    using SSP.Models.Account;
    using System.Threading.Tasks;

    public interface IAccountService
    {
        Task<UserSession> GetUserLoginAsync(string username);
    }
}