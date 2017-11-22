namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetFinancialReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "licensee")]
        public string Licensee { get; set; }

        [SpParam(Ignored = true)]
        public string LicenseeName { get; set; }

        [SpParam(SqlParamName = "datestart")]
        public DateTime DateStart { get; set; }

        [SpParam(SqlParamName = "dateend")]
        public DateTime DateEnd { get; set; }

        [SpParam(SqlParamName = "currency")]
        public string Currency { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyName { get; set; }

        [SpParam(SqlParamName = "gametype")]
        public string GameType { get; set; }

        [SpParam(Ignored = true)]
        public string GameTypeName { get; set; }

        [SpParam(SqlParamName = "gametable")]
        public string GameTable { get; set; }

        [SpParam(Ignored = true)]
        public string GameTableName { get; set; }

        public int TotalPlayers { get; set; }

        public int TotalGames { get; set; }

        public decimal GamingLevy { get; set; }

        public decimal ProRated { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameFinancial_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
