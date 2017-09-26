namespace TicketDesk.Web.Identity.Model
{
    using System.Collections.Generic;

    public class UniqueNameEmailDisplayUserEqualityComparer : IEqualityComparer<TicketDeskUser>
    {
        public bool Equals(TicketDeskUser x, TicketDeskUser y)
        {
            return x.DisplayName == y.DisplayName && x.Email == y.Email;
        }

        public int GetHashCode(TicketDeskUser obj)
        {
            return obj.DisplayName.GetHashCode() + obj.Email.GetHashCode();
        }
    }
}