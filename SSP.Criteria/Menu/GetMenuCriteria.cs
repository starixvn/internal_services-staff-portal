namespace SSP.Criteria.Menu
{
    using Fanex.Data.Repository;

    public class GetMenuCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Menu_Get";
    }
}