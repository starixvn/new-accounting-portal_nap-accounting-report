namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.MemberWinLossOnRNGCasino;
    using Fanex.AccountingReports.Misc.Models.MemberWinLossOnRNGCasino;
    using Fanex.Excel.Models;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMemberWinLossOnRNGCasinoService
    {
        List<MemberWinLoss> GetReportData(IMemberWinLossOnRNGCasinoCriteria criteria, bool isRecent);

        GridOptions GetRecentReport(IMemberWinLossOnRNGCasinoCriteria criteria);

        GridOptions GetHistoryReport(IMemberWinLossOnRNGCasinoCriteria criteria);

        IWorkbook GetRecentReportWorkbook(IMemberWinLossOnRNGCasinoCriteria criteria);

        IWorkbook GetHistoryReportWorkbook(IMemberWinLossOnRNGCasinoCriteria criteria);

        string GetReportName(IMemberWinLossOnRNGCasinoCriteria criteria);

        DateTime GetNextRefreshTime();

        DateTime GetLastRefreshTime();

        void SetLastRefreshTime(DateTime? time);
    }
}