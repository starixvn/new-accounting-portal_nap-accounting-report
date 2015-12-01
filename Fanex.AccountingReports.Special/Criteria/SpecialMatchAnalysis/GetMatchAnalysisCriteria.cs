// -----------------------------------------------------------------------
// <copyright file="GetMatchAnalysisCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchAnalysisCriteria : CriteriaBase
    {
        public GetMatchAnalysisCriteria()
        {
            Home = string.Empty;
            Away = string.Empty;
            League = string.Empty;
            Bettypes = string.Empty;
        }

        [SpParam(Ignored = true)]
        public string Home { get; set; }

        [SpParam(Ignored = true)]
        public string Away { get; set; }

        [SpParam(Ignored = true)]
        public string League { get; set; }

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

        [SpParam(Ignored = true)]
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
                case ReportType.Currency:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportByCurrency";
                    break;

                case ReportType.Bettype:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportByBetType";
                    break;

                case ReportType.Super:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportBySuper";
                    break;

                case ReportType.Member:
                    storeProcedureName = "Special_SpecialMatchAnalysis_GetReportByMember";
                    break;

                default:
                    storeProcedureName = string.Empty;
                    break;
            }

            return storeProcedureName;
        }
    }
}