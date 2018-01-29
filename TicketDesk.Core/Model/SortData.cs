namespace TicketDesk.Core.Model
{
    public class SortData
    {
        public string Column { get; set; }

        public bool IsAsc { get; set; }

        public int Priority { get; set; } = 1;
    }
}