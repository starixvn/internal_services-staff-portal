namespace SSP.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SSP.Models.Account;
    using SSP.Models.Function;
    using System.Threading.Tasks;

    public class AccountRepository : IAccountRepository
    {
        public async Task<UserSession> GetUserLogin(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria.GetSpParams()))
                {
                    var user = await multi.ReadFirstOrDefaultAsync<UserSession>();

                    if (user != null)
                    {
                        user.Functions = await multi.ReadAsync<Function>();
                    }

                    return user;
                }
            }
        }
    }
}