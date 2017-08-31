namespace SSP.Core.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Threading.Tasks;

    public class BaseRepository : IBaseRepository
    {
        public async Task<CommandResult> ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash.Add("errcode", 0);
                hash.Add("errmsg", string.Empty);

                await db.ExecuteNonQueryAsync(hash);
                CommandResult commandResult = GetCommandResult(hash);

                return commandResult;
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"] is DBNull;
            var errMsg = hash["errmsg"] is DBNull;

            return new CommandResult
            {
                Code = errCode ? 0 : Convert.ToInt32(errCode, CultureInfo.InvariantCulture),
                Message = errMsg ? string.Empty : Convert.ToString(errMsg, CultureInfo.InvariantCulture)
            };
        }
    }
}