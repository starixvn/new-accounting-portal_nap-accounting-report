namespace Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchAnalysisByUserLevelCriteria : CriteriaBase
    {
        public GetMatchAnalysisByUserLevelCriteria()
        {
            Bettypes = string.Empty;
        }

        [SpParam(Ignored = true)]
        public ReportType Reporttype { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        public DateTime BalanceUpDay
        {
            get
            {
                return ReportService.BalanceUp.BalanceUpDay;
            }
        }

        public int MatchId { get; set; }

        public int? LiveIndicator { get; set; }

        public string Bettypes { get; set; }

        public int StatusId { get; set; }

        public int CustId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            string storeProcedureName = string.Empty;
            switch (Reporttype)
            {
                case ReportType.SuperViewOnMaster:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportBySuper_SAg";
                    break;

                case ReportType.MasterViewOnAgent:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportBySuper_MAg";
                    break;

                case ReportType.AgentViewOnMember:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportBySuper_Ag";
                    break;

                default:
                    storeProcedureName = string.Empty;
                    break;
            }

            return storeProcedureName;
        }
    }
}