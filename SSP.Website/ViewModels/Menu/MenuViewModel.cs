namespace SSP.Website.ViewModels.Menu
{
    using SSP.Models.Function;
    using SSP.Models.Menu;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class MenuViewModel
    {
        public int MenuId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<Function> Functions { get; set; }

        public IEnumerable<Menu> Menus { get; set; }
    }
}