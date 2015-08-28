// -----------------------------------------------------------------------
// <copyright file="IInstallmentAmountService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.CreditTerm
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;

    /// <summary>
    /// The interface of Installment Amount Report.
    /// </summary>
    public interface IInstallmentAmountService
    {
        List<InstallAmount> GetInstallmentAmountReport(GetInstallAmountCriteria criteria);
    }
}
