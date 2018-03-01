namespace TicketDesk.Domain
{
    using Model;
    using System;
    using System.Security;
    using System.Text;
    using TicketDesk.Localization.Domain;

    public class TicketActionManager
    {
        internal TicketActionManager(TdDomainSecurityProviderBase securityProvider)
        {
            SecurityProvider = securityProvider;
        }

        private TdDomainSecurityProviderBase SecurityProvider { get; }

        // TODO: we should enforce required comments on the back-end
        public static TicketActionManager GetInstance(TdDomainSecurityProviderBase securityProvider)
        {
            return new TicketActionManager(securityProvider);
        }

        /// <summary>
        /// Determines whether the ticket activity is valid for the specified ticket.
        /// </summary>
        /// <remarks>
        /// This is a convienience method to provide a more natual api for the client.
        /// It just calls the equivalent method from the security provider.
        /// </remarks>
        /// <param name="ticket">The ticket.</param>
        /// <param name="activity">The activity.</param>
        /// <returns><c>true</c> if the ticket activity valid for the specified ticket; otherwise, <c>false</c>.</returns>
        public bool IsTicketActivityValid(Ticket ticket, TicketActivity activity)
        {
            return SecurityProvider.IsTicketActivityValid(ticket, activity);
        }

        /// <summary>
        /// Gets the valid ticket activities.
        /// </summary>
        /// <remarks>
        /// This is a convienience method to provide a more natual api for the client.
        /// It just calls the equivalent method from the security provider.
        /// </remarks>
        /// <param name="ticket">The ticket.</param>
        /// <returns>TicketActivity.</returns>
        public TicketActivity GetValidTicketActivities(Ticket ticket)
        {
            return SecurityProvider.GetValidTicketActivities(ticket);
        }

        public Action<Ticket> AddComment(string comment)
        {
            const TicketActivity activity = TicketActivity.AddComment;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> Assign(string comment, string assignTo, string priority)
        {
            return ChangeAssignment(comment, assignTo, priority, TicketActivity.Assign);
        }

        public Action<Ticket> CancelMoreInfo(string comment)
        {
            const TicketActivity activity = TicketActivity.CancelMoreInfo;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketStatus = TicketStatus.Active;

                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> Close(string comment)
        {
            const TicketActivity activity = TicketActivity.Close;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketStatus = TicketStatus.Closed;
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> EditTicketInfo(
            string comment,
            int projectId,
            string projectName,
            string title,
            string details,
            string priority,
            string ticketType,
            string category,
            string owner,
            string tagList,
            ApplicationSetting settings)
        {
            const TicketActivity activity = TicketActivity.EditTicketInfo;
            return ticket =>
            {
                if (!CheckSecurity(ticket, activity))
                {
                    return;
                }

                var sb = new StringBuilder(comment);
                sb.AppendLine();

                sb.AppendLine("<dl><dt>");
                sb.AppendLine(Strings.Changes_Title);
                sb.AppendLine("</dt>");

                AppendTitle(title, ticket, sb);
                AppendProjectId(projectId, projectName, ticket, sb);
                AppendDetails(details, ticket, sb);
                AppendTagList(tagList, settings, ticket, sb);
                AppendPriority(priority, settings, ticket, sb);
                AppendTicketType(ticketType, ticket, sb);
                AppendTicketCategory(category, ticket, sb);
                AppendOwner(owner, ticket, sb);

                sb.AppendLine("</dl>");
                ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, sb.ToString());
            };
        }

        private void AppendOwner(string owner, Ticket ticket, StringBuilder sb)
        {
            if (SecurityProvider.IsHostTicketCenter && ticket.Owner != owner)
            {
                sb.AppendLine(string.Format(
                    "<dd>    " + Strings.Changes_From_To + "</dd>",
                    PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.Owner),
                    SecurityProvider.GetUserDisplayName(ticket.Owner),
                    SecurityProvider.GetUserDisplayName(owner)));
                ticket.Owner = owner;
            }
        }

        private static void AppendTicketCategory(string category, Ticket ticket, StringBuilder sb)
        {
            if (ticket.Category != category)
            {
                sb.AppendLine(string.Format(
                    "<dd>    " + Strings.Changes_From_To + "</dd>",
                    PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.Category),
                    ticket.Category,
                    category));
                ticket.Category = category;
            }
        }

        private static void AppendTicketType(string ticketType, Ticket ticket, StringBuilder sb)
        {
            if (ticket.TicketType != ticketType)
            {
                sb.AppendLine(string.Format(
                    "<dd>    " + Strings.Changes_From_To + "</dd>",
                    PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.TicketType),
                    ticket.TicketType,
                    ticketType));
                ticket.TicketType = ticketType;
            }
        }

        private void AppendPriority(string priority, ApplicationSetting settings, Ticket ticket, StringBuilder sb)
        {
            if ((SecurityProvider.IsHostTicketCenter || settings.Permissions.AllowInternalUsersToEditPriority) && ticket.Priority != priority)
            {
                sb.AppendLine(string.Format(
                    "<dd>    " + Strings.Changes_From_To + "</dd>",
                    PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.Priority),
                    ticket.Priority,
                    priority));
                ticket.Priority = priority;
            }
        }

        private void AppendTagList(string tagList, ApplicationSetting settings, Ticket ticket, StringBuilder sb)
        {
            if ((SecurityProvider.IsHostTicketCenter || settings.Permissions.AllowInternalUsersToEditTags) && ticket.TagList != tagList)
            {
                sb.AppendLine(
                    $"<dd>    {PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.TagList)}</dd>");
                ticket.TagList = tagList;
            }
        }

        private static void AppendDetails(string details, Ticket ticket, StringBuilder sb)
        {
            if (ticket.Details != details)
            {
                sb.AppendLine(
                    $"<dd>    {PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.Details)}</dd>");
                ticket.Details = details;
            }
        }

        private static void AppendProjectId(int projectId, string projectName, Ticket ticket, StringBuilder sb)
        {
            if (ticket.ShopId != projectId)
            {
                sb.AppendLine(string.Format(
                    "<dd>    " + Strings.Changes_From_To + "</dd>",
                    PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.ShopId),
                    ticket.Shop.ShopName,
                    projectName));
                ticket.ShopId = projectId;
            }
        }

        private static void AppendTitle(string title, Ticket ticket, StringBuilder sb)
        {
            if (ticket.Title != title)
            {
                sb.AppendLine($"<dd>    {PropertyUtility.GetPropertyDisplayString<Ticket>(p => p.Title)}</dd>");
                ticket.Title = title;
            }
        }

        public Action<Ticket> ForceClose(string comment)
        {
            const TicketActivity activity = TicketActivity.ForceClose;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketStatus = TicketStatus.Closed;
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> GiveUp(string comment)
        {
            const TicketActivity activity = TicketActivity.GiveUp;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.AssignedTo = null;
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> ModifyAttachments(string comment)
        {
            // The vast majority of attachements management occur outside the business domain, so all we do here is log the activity event
            const TicketActivity activity = TicketActivity.ModifyAttachments;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> Pass(string comment, string assignTo, string priority)
        {
            return ChangeAssignment(comment, assignTo, priority, TicketActivity.Pass);
        }

        public Action<Ticket> ReAssign(string comment, string assignTo, string priority)
        {
            return ChangeAssignment(comment, assignTo, priority, TicketActivity.ReAssign);
        }

        public Action<Ticket> RequestMoreInfo(string comment)
        {
            const TicketActivity activity = TicketActivity.RequestMoreInfo;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketStatus = TicketStatus.MoreInfo;

                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> ReOpen(string comment, bool assignToMe)
        {
            const TicketActivity activity = TicketActivity.ReOpen;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.AssignedTo = assignToMe ? SecurityProvider.CurrentUserId : null;
                    ticket.TicketStatus = TicketStatus.Active;
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> Resolve(string comment)
        {
            const TicketActivity activity = TicketActivity.Resolve;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    ticket.TicketStatus = TicketStatus.Resolved;
                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> SupplyMoreInfo(string comment, bool reactivate)
        {
            const TicketActivity activity = TicketActivity.SupplyMoreInfo;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    if (reactivate)
                    {
                        ticket.TicketStatus = TicketStatus.Active;
                    }

                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment);
                }
            };
        }

        public Action<Ticket> TakeOver(string comment, string priority)
        {
            const TicketActivity activity = TicketActivity.TakeOver;
            return ticket =>
            {
                if (CheckSecurity(ticket, activity))
                {
                    var afterChangedPriority = priority;
                    ticket.AssignedTo = SecurityProvider.CurrentUserId;
                    if (!string.IsNullOrEmpty(afterChangedPriority))
                    {
                        if (ticket.Priority == afterChangedPriority)
                        {
                            afterChangedPriority = null;
                        }
                        else
                        {
                            ticket.Priority = afterChangedPriority;
                        }
                    }

                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment, afterChangedPriority, null);
                }
            };
        }

        private Action<Ticket> ChangeAssignment(string comment, string assignTo, string priority, TicketActivity activity)
        {
            return ticket =>
            {
                // attempting to assign/reassign to self
                if (SecurityProvider.CurrentUserId == assignTo)
                {
                    TakeOver(comment, priority)(ticket);
                }
                else
                {
                    if (!CheckSecurity(ticket, activity))
                    {
                        return;
                    }

                    var afterChangedPriority = priority;
                    ticket.AssignedTo = assignTo;
                    if (!string.IsNullOrEmpty(afterChangedPriority))
                    {
                        if (ticket.Priority == afterChangedPriority)
                        {
                            afterChangedPriority = null;
                        }
                        else
                        {
                            ticket.Priority = afterChangedPriority;
                        }
                    }

                    ticket.TicketEvents.AddActivityEvent(SecurityProvider.CurrentUserId, activity, comment, afterChangedPriority, SecurityProvider.GetUserDisplayName(assignTo));
                }
            };
        }

        private bool CheckSecurity(Ticket ticket, TicketActivity activity)
        {
            if (!IsTicketActivityValid(ticket, activity))
            {
                throw new SecurityException(Strings.CheckSecurity_NotAuthorized);
            }

            return true;
        }
    }
}