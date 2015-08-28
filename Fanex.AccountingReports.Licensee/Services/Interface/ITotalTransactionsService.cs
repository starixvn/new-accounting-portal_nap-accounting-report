// --------------------------------------------------------------------------
// <copyright file="ITotalTransactionsService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.31">Created</change>
//   <change who="Ricky.Nguyen" date="2012.11.5">Update Export Excel</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Criteria.TotalTransactions;
    using Fanex.AccountingReports.Licensee.Models.TotalTransactions;

    public interface ITotalTransactionsService
    {
        IEnumerable<Reseller> GetAllResellers();

        IEnumerable<ResellerRate> GetResellerRates(DateTime date);

        TransactionItem GetTotalTransactions(GetTotalTransactionsCriteria criteria);

        void Export(GetTotalTransactionsCriteria criteria);
    }
}
