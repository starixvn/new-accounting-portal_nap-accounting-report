namespace Fanex.AccountingReports.Racing.Services.Implement
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Racing.Criteria.ForTote;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class ToteDailyWinLossAbstractExcelService : ServiceBase
    {
        public abstract void ExportExcelRacingDailyMain(GetMainForToteCriteria criteria);

        public abstract void ExportExcelRacingDailyDetail(GetDetailForToteCriteria criteria);

        public virtual string[] GetReportName()
        { 
            return null; 
        }
    }
}
