namespace TicketDesk.Web.Client.Areas.Admin.Models.TicketList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Criteria.TicketList;
    using Domain.Model;
    using Localization.Client;

    public class TicketListFilterViewModel
    {
        [Display(Name = nameof(Resources.ShopName), ResourceType = typeof(Resources))]
        public int? ShopId { get; set; }

        public IEnumerable<SelectListItem> Shops { get; set; }

        [Display(Name = nameof(Resources.Type), ResourceType = typeof(Resources))]
        public TicketCategoryTypes? TicketType { get; set; }

        public TicketStatus? Status { get; set; }

        [Display(Name = nameof(Resources.TicketListFilterViewModel_CreateDateFrom), ResourceType = typeof(Resources))]
        public DateTime? CreateDateFrom { get; set; }

        [Display(Name = nameof(Resources.TicketListFilterViewModel_CreateDateTo), ResourceType = typeof(Resources))]
        public DateTime? CreateDateTo { get; set; }

        public TicketListFilterCriteria GetQueryCriteria()
        {
            return new TicketListFilterCriteria
            {
                ShopId = ShopId,
                TicketType = TicketType,
                Status = Status,
                FromDate = CreateDateFrom,
                ToDate = CreateDateTo
            };
        }
    }
}