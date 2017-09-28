namespace TicketDesk.Core.Model
{
    using Resources;

    public class CommandResult : Fanex.NAP.Core.Command.CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Messages.InvalidInputData;
        }
    }
}