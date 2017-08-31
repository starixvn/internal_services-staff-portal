namespace SSP.Models.Menu
{
    using SSP.Models.Function;
    using System;

    [Serializable]
    public class Menu
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public string Link { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public Function Function { get; set; }

        public Menu ParentMenu { get; set; }
    }
}