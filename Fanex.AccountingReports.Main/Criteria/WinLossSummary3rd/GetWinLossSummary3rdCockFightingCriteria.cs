namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using Fanex.Data.Repository;

    public class GetWinLossSummary3rdCockFightingCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_WinLossSummary3rd_GetCockFightingReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}