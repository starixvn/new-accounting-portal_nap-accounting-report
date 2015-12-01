// -----------------------------------------------------------------------
// <copyright file="GetMainForToteCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Criteria.ForTote
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMainForToteCriteria : CriteriaBase
    {
        public int ReportType { get; set; }

        [SpParam(SqlParamName = "winlostdate")]
        public DateTime WinlostDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            if (WinlostDate > ReportService.BalanceUp.MoveBetsDay)
            {
                return "Racing_HorseRacingDailyReportForTote";
            }
            else
            {
                return "Racing_HorseRacingDailyReportForTote_14";
            }
        }
    }
}