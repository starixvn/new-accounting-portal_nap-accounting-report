namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using Fanex.AccountingReports.Licensee.Criteria.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INegativeBanlance
    {
        NegativeBalanceModel GetNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria);

        void ExportExcelNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria);

        NegativeBalanceMemberSendMailFail SendMail(MailParam mailParam, NegativeBalanceCriteria negativeBalanceCriteria);
    }
}
