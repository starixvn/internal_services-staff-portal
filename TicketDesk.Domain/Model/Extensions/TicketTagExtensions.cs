namespace TicketDesk.Domain.Model
{
    using System.Collections.Generic;

    public static class TicketTagExtensions
    {
        public static void AddRange(this ICollection<TicketTag> set, IEnumerable<TicketTag> tags)
        {
            foreach (var tag in tags)
            {
                set.Add(tag);
            }
        }
    }
}