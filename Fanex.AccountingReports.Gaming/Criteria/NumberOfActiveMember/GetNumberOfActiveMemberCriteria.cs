namespace Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember
{
    using System;
    using Fanex.Data.Repository;

    public class GetNumberOfActiveMemberCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public string ProductList { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_NumberOfActiveMember_GetReport";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}
