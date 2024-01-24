namespace Fanex.AccountingReports.Special.Criteria.SaveBetReport
{
    using System;
    using Fanex.Data.Repository;

    public class GetSportsCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Special_SaveBet_GetListSport";
        }
    }
}
