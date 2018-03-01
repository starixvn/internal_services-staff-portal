namespace TicketDesk.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using TicketDesk.Localization;
    using TicketDesk.Localization.Domain;

    public class Ticket
    {
        private string _owner;

        private string _assignedTo;

        public Ticket()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            TicketEvents = new HashSet<TicketEvent>();
            TicketSubscribers = new HashSet<TicketSubscriber>();
            TicketTags = new HashSet<TicketTag>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
            var none = "empty";
            ////Title = none;
            TicketType = none;
            Category = none;
            Details = none;
        }

        [Key]
        [Display(ResourceType = typeof(Strings), Name = "TicketTicketId", ShortName = "TicketTicketIdShort")]
        public int TicketId { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(ResourceType = typeof(Strings), Name = "TicketProject", ShortName = "TicketProjectShort")]
        public int ShopId { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(50, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketTicketType", ShortName = "TicketTicketTypeShort")]
        public string TicketType { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(50, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketCategory", ShortName = "TicketCategoryShort")]
        public string Category { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(500, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketTitle", ShortName = "TicketTitleShort")]
        public string Title { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketDetails", ShortName = "TicketDetailsShort")]
        public string Details { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "TicketIsHtml", ShortName = "TicketIsHtmlShort")]
        public bool IsHtml { get; set; }

        [StringLength(100, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketTagList", ShortName = "TicketTagListShort")]
        public string TagList { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketCreatedBy", ShortName = "TicketCreatedByShort")]
        public string CreatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(ResourceType = typeof(Strings), Name = "TicketCreatedDate", ShortName = "TicketCreatedDateShort")]
        public DateTimeOffset CreatedDate { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketOwner", ShortName = "TicketOwnerShort")]
        public string Owner
        {
            get
            {
                return _owner;
            }

            set
            {
                PreviousOwner = _owner;
                _owner = value;
            }
        }

        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketAssignedTo", ShortName = "TicketAssignedToShort")]
        public string AssignedTo
        {
            get
            {
                return _assignedTo;
            }

            set
            {
                PreviousAssignedUser = _assignedTo;
                _assignedTo = value;
            }
        }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketTicketStatus", ShortName = "TicketTicketStatusShort")]
        public TicketStatus TicketStatus { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "TicketCurrentStatusDate", ShortName = "TicketCurrentStatusDateShort")]
        public DateTimeOffset CurrentStatusDate { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketCurrentStatusSetBy", ShortName = "TicketCurrentStatusSetByShort")]
        public string CurrentStatusSetBy { get; set; }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Validation))]
        [StringLength(256, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketLastUpdateBy", ShortName = "TicketLastUpdateByShort")]
        public string LastUpdateBy { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "TicketLastUpdateDate", ShortName = "TicketLastUpdateDateShort")]
        public DateTimeOffset LastUpdateDate { get; set; }

        [StringLength(25, ErrorMessageResourceName = "FieldMaximumLength", ErrorMessageResourceType = typeof(Validation))]
        [Display(ResourceType = typeof(Strings), Name = "TicketPriority", ShortName = "TicketPriorityShort")]
        public string Priority { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "TicketAffectsCustomer", ShortName = "TicketAffectsCustomerShort")]
        public bool AffectsCustomer { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        public virtual Shop Shop { get; set; }

        public int CategoryId { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }

        public virtual ICollection<TicketEvent> TicketEvents { get; set; }

        public virtual ICollection<TicketTag> TicketTags { get; set; }

        public virtual ICollection<TicketSubscriber> TicketSubscribers { get; set; }

        [NotMapped]
        internal string PreviousOwner { get; set; }

        [NotMapped]
        internal string PreviousAssignedUser { get; set; }

        [NotMapped]
        public bool IsAssigned => !string.IsNullOrEmpty(AssignedTo);

        [NotMapped]
        public bool IsOpen => TicketStatus != TicketStatus.Resolved && TicketStatus != TicketStatus.Closed;

        public void EnsureSubscribers()
        {
            EnsureSubscriber(Owner);
            EnsureSubscriber(AssignedTo);
            EnsureSubscriber(PreviousOwner);
            EnsureSubscriber(PreviousAssignedUser);
        }

        private void EnsureSubscriber(string user)
        {
            if (user != null && TicketSubscribers.All(s => s.SubscriberId != user))
            {
                TicketSubscribers.Add(new TicketSubscriber { SubscriberId = user });
            }
        }

        public TicketActivity GetAvailableActivites(string userId)
        {
            var isOwnedByMe = Owner == userId;
            var isMoreInfo = TicketStatus == TicketStatus.MoreInfo;
            var isAssignedToMe = !string.IsNullOrEmpty(AssignedTo) && AssignedTo == userId;
            var isResolved = TicketStatus == TicketStatus.Resolved;
            var validActivities = TicketActivity.None;

            if (TicketId == default(int))
            {
                validActivities |= TicketActivity.Create | TicketActivity.CreateOnBehalfOf;
            }

            validActivities = GetIsOpen(isOwnedByMe, isMoreInfo, isAssignedToMe, validActivities);

            if (isResolved && isOwnedByMe)
            {
                validActivities |= TicketActivity.Close;
            }

            validActivities = GetIsOpenOrIsResolved(isOwnedByMe, isAssignedToMe, isResolved, validActivities);

            return validActivities;
        }

        private TicketActivity GetIsOpenOrIsResolved(bool isOwnedByMe, bool isAssignedToMe, bool isResolved, TicketActivity ticketActivities)
        {
            var validActivities = ticketActivities;

            if (IsOpen || isResolved)
            {
                if (IsAssigned)
                {
                    if (!isAssignedToMe)
                    {
                        validActivities |= TicketActivity.ReAssign;
                    }
                }
                else
                {
                    // !assigned
                    validActivities |= TicketActivity.Assign;
                }

                if ((isAssignedToMe || isOwnedByMe) && !(isResolved && isOwnedByMe))
                {
                    validActivities |= TicketActivity.ForceClose;
                }

                validActivities |=
                    isAssignedToMe ? TicketActivity.Pass | TicketActivity.GiveUp : TicketActivity.TakeOver;
            }

            return validActivities;
        }

        private TicketActivity GetIsOpen(bool isOwnedByMe, bool isMoreInfo, bool isAssignedToMe, TicketActivity ticketActivities)
        {
            var validActivities = ticketActivities;

            if (IsOpen)
            {
                validActivities |= TicketActivity.ModifyAttachments;

                if (isOwnedByMe || isAssignedToMe)
                {
                    validActivities |= TicketActivity.EditTicketInfo;
                }

                if (isMoreInfo)
                {
                    validActivities |= TicketActivity.SupplyMoreInfo;
                    if (isAssignedToMe)
                    {
                        validActivities |= TicketActivity.CancelMoreInfo;
                    }
                }
                else
                {
                    // !moreInfo
                    validActivities |= TicketActivity.AddComment;
                    if (isAssignedToMe)
                    {
                        validActivities |= TicketActivity.Resolve | TicketActivity.RequestMoreInfo;
                    }
                }
            }
            else
            {
                // not open (resolved or closed)
                validActivities |= TicketActivity.ReOpen;
            }

            return validActivities;
        }

        /// <summary>
        /// Performs an activity function on the ticket.
        /// </summary>
        /// <param name="ticketAction">The ticket action to perform.</param>
        public void PerformAction(Action<Ticket> ticketAction)
        {
            ticketAction(this);
        }
    }
}