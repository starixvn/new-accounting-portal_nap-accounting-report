namespace Fanex.AccountingReports.WinLoss.Services.Interface.TreeWinLoss
{
    using Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss;

    /// <summary>
    /// Create interface Tree Win Loss Excel Service.
    /// </summary>
    public interface ITreeWinLossExcelService
    {
        void ExportExcelTreeWinLossByProduct(GetTreeWinLossByProductCriteria criteria);

        void ExportExcelTreeWinLossByLevel(GetTreeWinLossByLevelCriteria criteria);
    }
}