namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Special.Criteria.SMATurnover;
    using Fanex.AccountingReports.Special.Models.SMATurnover;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISMATurnoverService
    {
        List<EntityBase<int, string>> GetAccountStatus();

        List<BaseModel> GetProducts();

        SMATurnover GetSMAsTurnover(SMATurnoverCriteria criteria);

        SMATurnover SortReport(SMATurnoverCriteria criteria);

        void ExportReportData(SMATurnoverCriteria criteria);
    }
}
