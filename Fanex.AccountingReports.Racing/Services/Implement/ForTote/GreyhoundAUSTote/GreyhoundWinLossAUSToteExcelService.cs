namespace Fanex.AccountingReports.Racing.Services.Implement.ForTote.GreyhoundAUSTote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GreyhoundWinLossAUSToteExcelService : ToteDailyWinLossExcelService
    {
        public override string[] GetReportName()
        {
            var reportType = new string[2];
            reportType[0] = RacingResources.Greyhound;
            reportType[1] = RacingResources.AUS;
            return reportType;
        }
    }
}
