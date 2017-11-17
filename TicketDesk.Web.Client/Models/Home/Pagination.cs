namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class Pagination
    {
        /// <summary>
        /// CurrentPage start with 1
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// If current page = 1 (first page) => From item 1
        /// If current page = 2 and ItemPerPage = 20 => From item 21
        /// </summary>
        public virtual int FromItemIndex => CurrentPage == 1 ? 1 : ((CurrentPage - 1) * ItemPerPage) + 1;

        public virtual int ToItemIndex => CurrentPage != TotalPage ? ItemPerPage * CurrentPage : TotalItem;

        [Display(Name = "Page size")]
        public int ItemPerPage { get; set; }

        public virtual IEnumerable<SelectListItem> ItemPerPageList
        {
            get
            {
                List<SelectListItem> selectListItems = new List<SelectListItem>();

                SelectListItem selectListItem = new SelectListItem
                {
                    Text = "20",
                    Value = "20"
                };
                selectListItems.Add(selectListItem);

                selectListItem = new SelectListItem
                {
                    Text = "40",
                    Value = "40"
                };
                selectListItems.Add(selectListItem);

                selectListItem = new SelectListItem
                {
                    Text = "60",
                    Value = "60"
                };
                selectListItems.Add(selectListItem);

                return selectListItems;
            }
        }

        public int TotalItem { get; set; }

        public virtual int TotalPage => TotalItem % ItemPerPage > 0
            ? (TotalItem / ItemPerPage) + 1
            : TotalItem / ItemPerPage;

        public string Action { get; set; }

        public string Controller { get; set; }
    }
}
