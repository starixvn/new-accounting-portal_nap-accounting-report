// -----------------------------------------------------------------------
// <copyright file="ICreditTermExportExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.CreditTerm
{
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;

    /// <summary>
    /// The export excel of Credit Term report.
    /// </summary>
    public interface ICreditTermExportExcelService
    {
        void ExportCreditTermReport(GetCreditTermCriteria criteria);
    }
}
