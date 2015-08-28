// --------------------------------------------------------------------------
// <copyright file="IDirectBalanceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Criteria.DirectBalance;
    using Fanex.AccountingReports.Balance.Models.DirectBalance;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDirectBalanceService
    {
        DirectBalance GetDirectBalance(GetDirectBalanceCriteria directBalanceCriteria);

        DirectBalance GetDirectBalanceMasterDownLine(GetDirectBalanceMasterDownlineCriteria directBalanceMasterDownlineCriteria);

        DirectBalance GetDirectBalanceAgentDownLine(GetDirectBalanceAgentDownlineCriteria directBalanceAgentDownlineCriteria);

        DirectBalance ExportDirectBalanceReport(GetDirectBalanceCriteria directBalanceCriteria);

        DirectBalance ExportBalanceMasterDownLineReport(GetDirectBalanceMasterDownlineCriteria getDirectBalanceMasterCriteria);

        DirectBalance ExportBalanceAgentDownLineReport(GetDirectBalanceAgentDownlineCriteria getDirectBalanceAgentCriteria);

        List<EntityBase<int, string>> GetCurrencies();
    }
}
