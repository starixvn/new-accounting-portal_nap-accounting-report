namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Misc.Criteria.SMATurnover;
    using Fanex.AccountingReports.Misc.Models.SMATurnover;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISMATurnoverService
    {
        List<EntityBase<int, string>> GetAccountStatus();

        List<BaseModel> GetProducts();

        SMATurnover GetSMAsTurnover(GetSMATurnoverReportCriteria criteria);

        SMATurnover SortReport(GetSMATurnoverReportCriteria criteria);

        void ExportReportData(GetSMATurnoverReportCriteria criteria);
    }
}
