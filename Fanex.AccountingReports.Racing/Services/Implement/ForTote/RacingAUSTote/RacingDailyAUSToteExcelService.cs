// -----------------------------------------------------------------------
// <copyright file="RacingDailyAUSToteExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingAUSTote
{
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// Create racing daily AUS Tote excel service.
    /// </summary>
    public class RacingDailyAUSToteExcelService : ToteDailyWinLossExcelService
    {
        public override string[] GetReportName()
        {
            var reportType = new string[2];
            reportType[0] = RacingResources.Racing;
            reportType[1] = RacingResources.AUS;
            return reportType;
        }
    }
}
