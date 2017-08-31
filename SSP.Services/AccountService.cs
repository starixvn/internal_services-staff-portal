namespace SSP.Services
{
    using SSP.Criteria.Account;
    using SSP.Models.Account;
    using SSP.Repositories;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;

        public AccountService(IAccountRepository accountRepository = null)
        {
            this.accountRepository = accountRepository ?? new AccountRepository();
        }

        public async Task<UserSession> GetUserLogin(string username)
        {
            var criteria = new GetUserLoginByUsernameCriteria
            {
                Username = username
            };
            var user = new UserSession();

            if (criteria.IsValid())
            {
                user = await accountRepository.GetUserLogin(criteria);
            }

            return user;
        }
    }
}