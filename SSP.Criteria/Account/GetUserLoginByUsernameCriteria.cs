namespace SSP.Criteria.Account
{
    using Fanex.Data.Repository;

    public class GetUserLoginByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);

        public override string GetSettingKey() => "Account_GetByUsername";
    }
}