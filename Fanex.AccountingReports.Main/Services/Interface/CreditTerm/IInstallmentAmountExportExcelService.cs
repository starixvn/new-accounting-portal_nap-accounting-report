// -----------------------------------------------------------------------
// <copyright file="IInstallmentAmountExportExcelService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.CreditTerm
{
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;

    /// <summary>
    /// The export excel service of Installment Amount report.
    /// </summary>
    public interface IInstallmentAmountExportExcelService
    {
        void ExportInstallmentAmountReport(GetInstallAmountCriteria criteria);
    }
}
