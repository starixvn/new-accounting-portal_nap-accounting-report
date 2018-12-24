namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using Fanex.Data.Repository;

    public class GetWinLossSummary3rdNumberGameSportTypeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }


        public override string GetSettingKey()
        {
            return "Main_WinLossSummary3rd_Get_Sporttype_NG";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
