namespace SSP.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class DeleteMenuCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public override string GetSettingKey() => "Menu_Delete";

        public override bool IsValid() => MenuId > 0;
    }
}