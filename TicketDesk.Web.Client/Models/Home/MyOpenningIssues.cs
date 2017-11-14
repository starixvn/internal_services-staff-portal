namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class MyOpenningIssues
    {
        /// <summary>
        /// CurrentPage start with 1
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// If current page = 1 (first page) => From item 1
        /// If current page = 2 and ItemPerPage = 20 => From item 21
        /// </summary>
        public int FromItemIndex => CurrentPage == 1 ? 1 : ((CurrentPage - 1) * ItemPerPage) + 1;

        public int ToItemIndex => CurrentPage != TotalPage ? ItemPerPage * CurrentPage : TotalItem;

        public int ItemPerPage { get; set; }

        public IEnumerable<SelectListItem> ItemPerPageList
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

        public IList<TicketGroup> TicketGroups { get; set; }

        public int TotalItem { get; set; }

        public int TotalPage => TotalItem % ItemPerPage > 0
            ? (TotalItem / ItemPerPage) + 1
            : TotalItem / ItemPerPage;
    }
}