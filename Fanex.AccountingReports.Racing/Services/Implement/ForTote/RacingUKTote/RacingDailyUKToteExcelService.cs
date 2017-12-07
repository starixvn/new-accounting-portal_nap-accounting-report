namespace Fanex.AccountingReports.Racing.Services.Implement.RacingUKTote
{
    using Fanex.AccountingReports.Racing.Resources;

    /// <summary>
    /// Create Racing Daily Excel Service.
    /// </summary>
    public class RacingDailyUKToteExcelService : ToteDailyWinLossExcelService
    {
        public override string[] GetReportName()
        {
            var reportType = new string[2];
            reportType[0] = RacingResources.Racing;
            reportType[1] = RacingResources.UK;
            return reportType;
        }
    }
}
