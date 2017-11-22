namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Models.WinLossSummary;
    using Fanex.Data.Repository;

    public interface IWinLossSummaryService
    {
        WinLossInfo GetReport(CriteriaBase criteria);
    }
}
