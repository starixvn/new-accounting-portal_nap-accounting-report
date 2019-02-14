namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using Data;
    using Fanex.Data.Repository;

    public class GetWinLossSummary3rdSportbookSportTypeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public int LiveIndicator { get; set; }

        [SpParam(SqlParamName = "drilldownbytype")]
        public int DrillDownByType { get; set; }

        public string SubProduct { get; set; }

        public override string GetSettingKey()
        {
            return "Main_WinLossSummary3rd_Get_Sporttype_SB";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
