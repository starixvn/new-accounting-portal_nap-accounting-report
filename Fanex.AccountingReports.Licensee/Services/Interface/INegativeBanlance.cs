// --------------------------------------------------------------------------
// <copyright file="INegativeBanlance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.20">Created</change>
// </history>

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
