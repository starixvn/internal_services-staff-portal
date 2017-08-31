namespace SSP.Criteria.Function
{
    using Fanex.Data.Repository;

    public class GetFunctionCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Function_Get";
    }
}