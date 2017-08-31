namespace SSP.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class GetMenuByIdCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public override bool IsValid() => MenuId > 0;

        public override string GetSettingKey() => "Menu_GetById";
    }
}