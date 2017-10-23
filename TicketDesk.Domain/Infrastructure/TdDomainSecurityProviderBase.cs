namespace TicketDesk.Domain
{
    using Model;
    using System;
    using System.Security;

    /// <summary>
    /// Provides the minimal set of security services to support business logic
    /// </summary>
    public abstract class TdDomainSecurityProviderBase
    {
        public abstract Func<string, string> GetUserDisplayName { get; }

        public abstract string CurrentUserId { get; }

        public string CurrentUserDisplayName => GetUserDisplayName(CurrentUserId);

        public abstract bool IsTdHelpDeskUser
        {
            get;
        }

        public abstract bool IsTdInternalUser
        {
            get;
        }

        public abstract bool IsTdAdministrator
        {
            get;
        }

        public abstract bool IsTdPendingUser
        {
            get;
        }

        public abstract IContextHelper NapContextHelper { get; set; }

        public abstract int NapCurrentUserId { get; }

        public abstract bool IsReadTicketCenter { get; }

        public abstract bool IsHostTicketCenter { get; }

        public abstract bool IsReadNewTicket { get; }

        internal static TicketActivity GetReadTicketActivityPermission()
        {
            return TicketActivity.ModifyAttachments |
                TicketActivity.SupplyMoreInfo |
                TicketActivity.ReOpen |
                TicketActivity.AddComment |
                TicketActivity.Close |
                TicketActivity.ForceClose;
        }

        internal static TicketActivity GetHostTicketActivityPermission()
        {
            return GetReadTicketActivityPermission() |
                TicketActivity.RequestMoreInfo |
                TicketActivity.CancelMoreInfo |
                TicketActivity.CreateOnBehalfOf |
                TicketActivity.TakeOver |
                TicketActivity.Assign |
                TicketActivity.ReAssign |
                TicketActivity.Pass |
                TicketActivity.GiveUp |
                TicketActivity.Resolve;
        }

        internal static TicketActivity GetNewTicketPermission()
        {
            return TicketActivity.EditTicketInfo | TicketActivity.Create;
        }

        public TicketActivity GetValidTicketActivities(Ticket ticket)
        {
            // Role
            if (!(IsReadTicketCenter || IsHostTicketCenter || IsReadNewTicket))
            {
                throw new SecurityException("User is not authorized to read ticket data.");
            }

            var validTicketActivities = ticket.GetAvailableActivites(CurrentUserId);

            var allowedActivities = TicketActivity.None;

            if (IsReadNewTicket)
            {
                allowedActivities |= GetNewTicketPermission();
            }

            if (IsReadTicketCenter)
            {
                allowedActivities |= GetReadTicketActivityPermission();
            }

            if (IsHostTicketCenter)
            {
                allowedActivities |= GetHostTicketActivityPermission();
            }

            return validTicketActivities & allowedActivities;
        }

        public bool IsTicketActivityValid(Ticket ticket, TicketActivity activity)
        {
            return GetValidTicketActivities(ticket).HasFlag(activity);
        }
    }
}