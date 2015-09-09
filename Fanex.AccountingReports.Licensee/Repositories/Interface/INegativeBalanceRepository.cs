// --------------------------------------------------------------------------
// <copyright file="INegativeBalanceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INegativeBalanceRepository
    {
        List<NegativeBalanceUser> GetNegativeBalanceUser(ICriteria criteria);
    }
}