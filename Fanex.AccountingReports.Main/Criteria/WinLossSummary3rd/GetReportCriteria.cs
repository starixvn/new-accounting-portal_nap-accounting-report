namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_WinLossSummary3rd_GetReport";
        }

        public override bool IsValid()
        {
            //// Fromdate.AddMonths(3) <= Todate;
            return true;
        }
    }
}
