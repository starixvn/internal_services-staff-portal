namespace TicketDesk.Criteria.Shop
{
    using System.ComponentModel.DataAnnotations;
    using Core.Enumerations;

    public class ShopFilter
    {
        [Display(Name = "Shop Name")]
        [StringLength(100)]
        public string ShopName { get; set; }

        public int ShopId { get; set; }

        [Display(Name = "Active")]
        public ActiveState ActiveState { get; set; }

        public bool IsHostPermission { get; set; }
    }
}