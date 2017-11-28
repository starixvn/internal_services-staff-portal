namespace TicketDesk.Domain.Extensions
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Model;

    public static class TicketExtensions
    {
        public static int GetTitleMaxLength(this Ticket ticket)
        {
            var property =typeof(Ticket).GetProperty("Title");
            var attribute = (StringLengthAttribute)property?.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault();

            return attribute?.MaximumLength ?? 0;
        }
    }
}
