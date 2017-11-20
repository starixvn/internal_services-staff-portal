namespace TicketDesk.Core.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections;
    using System.Globalization;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable param = criteria.GetSpParams();
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);

                db.ExecuteNonQuery(param);
                CommandResult commandResult = GetCommandResult(param);

                return commandResult;
            }
        }

        private static CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];
            var errorCode = errCode is DBNull;

            return new CommandResult
            {
                Code = errorCode ? 0 : Convert.ToInt32(errCode, CultureInfo.InvariantCulture),
                Message = errorCode ? string.Empty : Convert.ToString(errMsg, CultureInfo.InvariantCulture)
            };
        }
    }
}