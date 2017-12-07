namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;

    /// <summary>
    /// The interface of Credit Term repository.
    /// </summary>
    public interface ICreditTermRepository
    {
        List<CreditTerm> GetCreditTerm(GetCreditTermCriteria criteria);

        EditedRemarkResult EditRemark(GetRemarkEditedCriteria criteria);
    }
}
