namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;
    using Fanex.AccountingReports.Main.Repositories.Implement;

    /// <summary>
    /// The get report service of Credit Term report.
    /// </summary>
    public class CreditTermService : ServiceBase
    {
        private CreditTermRepository _creditTermRepository;

        public CreditTermService()
        {
            this._creditTermRepository = new CreditTermRepository();
        }

        /// <summary>
        /// Get Credit Term report.
        /// </summary>
        /// <param name="criteira">The criteria.</param>
        /// <returns>The list Credit Term.</returns>
        public List<CreditTerm> GetCreditTermReport(GetCreditTermCriteria criteira)
        {
            try
            {
                var report = this._creditTermRepository.GetCreditTerm(criteira);

                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Save and write log edited remark.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The GetRemarkEditedCriteria.</returns>
        public EditedRemarkResult EditRemark(GetRemarkEditedCriteria criteria)
        {
            try
            {
                var editRemark = new EditedRemarkResult();

                if (AppSession.CurrentUser != null)
                {
                    criteria.UserID = AppSession.CurrentUser.Id;
                }

                editRemark = this._creditTermRepository.EditRemark(criteria);

                return editRemark;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }
    }
}