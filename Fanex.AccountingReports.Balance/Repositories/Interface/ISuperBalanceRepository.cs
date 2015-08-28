// -----------------------------------------------------------------------
// <copyright file="ISuperBalanceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="" date="2013.30.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISuperBalanceRepository
    {
        List<dynamic> GetSuperBalance(GetSuperBalanceCriteria criteria, ref Hashtable outputParam);

        List<SuperBalance> GetSuperBalanceDownline(ICriteria criteria, ref Hashtable outputParam);

        List<SuperBalance> GetSuperBalanceMasterDownline(ICriteria criteria, ref Hashtable outputParam);
    }
}
