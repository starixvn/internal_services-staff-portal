namespace TicketDesk.Web.Client.Models.Home
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class HomeViewModel
    {
        public IList<TicketGroup> TicketGroups { get; set; }

        public int TotalPage { get; set; }

        public int CurrentPage { get; set; }

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
    }
}