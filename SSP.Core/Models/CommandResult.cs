namespace SSP.Core.Models
{
    using Resources;

    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = Messages.InvalidInputData;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;
    }
}