// -----------------------------------------------------------------------
// <copyright file="ICreditTermService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.CreditTerm
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;

    /// <summary>
    /// The interface of get report service of Credit Term report.
    /// </summary>
    public interface ICreditTermService
    {
        List<CreditTerm> GetCreditTermReport(GetCreditTermCriteria criteria);

        EditedRemarkResult EditRemark(GetRemarkEditedCriteria criteira);
    }
}
