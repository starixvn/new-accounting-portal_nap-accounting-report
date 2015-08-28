// -----------------------------------------------------------------------
// <copyright file="RacingDailySAToteExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingSATote
{
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RacingDailySAToteExcelService : ToteDailyWinLossExcelService
    {
        public override string[] GetReportName()
        {
            var reportType = new string[2];
            reportType[0] = RacingResources.Racing;
            reportType[1] = RacingResources.SA;
            return reportType;
        }
    }
}
