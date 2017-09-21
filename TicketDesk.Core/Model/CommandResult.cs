namespace TicketDesk.Core.Model
{
    public class CommandResult : Fanex.NAP.Core.Command.CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Messages.Messages.InvalidInputData;
        }
    }
}