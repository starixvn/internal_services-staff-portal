namespace TicketDesk.Web.Client.Models.Common
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Localization.Client;

    public abstract class Pagination
    {
        /// <summary>
        /// CurrentPage start with 1
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// If current page = 1 (first page) => From item 1
        /// If current page = 2 and ItemPerPage = 20 => From item 21
        /// </summary>
        public int FromItemIndex => CurrentPage == 1 ? 1 : ((CurrentPage - 1) * ItemPerPage) + 1;

        public int ToItemIndex => CurrentPage != TotalPage ? ItemPerPage * CurrentPage : TotalItem;

        [Display(ResourceType = typeof(Resources), Name = "Common_Table_Pagination_PageSize")]
        public int ItemPerPage { get; set; } = 20;

        public IEnumerable<SelectListItem> ItemPerPageList
        {
            get
            {
                var selectListItems = new List<SelectListItem>();

                var selectListItem = new SelectListItem
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

        public virtual IList<int> PageSizes => new List<int> { 20, 40, 60 };

        public int TotalItem { get; set; }

        public int TotalPage => TotalItem % ItemPerPage > 0
            ? (TotalItem / ItemPerPage) + 1
            : TotalItem / ItemPerPage;

        public string Action { get; set; }

        public string Controller { get; set; }

        public int Skipped
        {
            get
            {
                var databasePage = CurrentPage < 1 ? 0 : CurrentPage - 1;

                var skippedItem = databasePage * ItemPerPage;

                if (skippedItem < 0)
                {
                    skippedItem = 0;
                }

                return skippedItem;
            }
        }

        public virtual string NextLink()
        {
            return IndexLink(CurrentPage + 1, ItemPerPage);
        }

        public virtual string PreviousLink()
        {
            return IndexLink(CurrentPage - 1, ItemPerPage);
        }

        public abstract string IndexLink(int i, int itemPerPage);

        public virtual string PageSizeLink(int itemPerPage)
        {
            return IndexLink(1, itemPerPage);
        }
    }
}