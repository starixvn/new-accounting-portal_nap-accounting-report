// -----------------------------------------------------------------------
// <copyright file="GetBettradeCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetBettradeCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2h_GetBettradeReport" + ReportService.GetStoreMappingSuffix(this.WinlostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}