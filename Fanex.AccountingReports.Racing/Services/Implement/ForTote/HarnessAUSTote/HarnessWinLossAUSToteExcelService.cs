// -----------------------------------------------------------------------
// <copyright file="HarnessWinLossAUSToteExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.ForTote.HarnessAUSTote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HarnessWinLossAUSToteExcelService : ToteDailyWinLossExcelService
    {
        public override string[] GetReportName()
        {
            var reportType = new string[2];
            reportType[0] = RacingResources.Harness;
            reportType[1] = RacingResources.AUS;
            return reportType;
        }
    }
}
