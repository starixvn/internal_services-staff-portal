namespace TicketDesk.Web.Identity.Model
{
    using System.Collections.Generic;

    public class StandardUserEqualityComparer : IEqualityComparer<TicketDeskUser>
    {
        public bool Equals(TicketDeskUser x, TicketDeskUser y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(TicketDeskUser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}