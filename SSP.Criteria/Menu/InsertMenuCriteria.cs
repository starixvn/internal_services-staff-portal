namespace SSP.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class InsertMenuCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "Menu_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name);
    }
}