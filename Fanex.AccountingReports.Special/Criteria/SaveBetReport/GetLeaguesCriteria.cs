namespace Fanex.AccountingReports.Special.Criteria.SaveBetReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeaguesCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public int SportType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Special_SaveBet_GetListLeague";
        }
    }
}
