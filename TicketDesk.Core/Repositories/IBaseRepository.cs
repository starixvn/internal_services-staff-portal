namespace TicketDesk.Core.Repositories
{
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseRepository
    {
        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}