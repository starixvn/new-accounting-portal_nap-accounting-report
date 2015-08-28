// --------------------------------------------------------------------------
// <copyright file="ICreditSitesBetCountsService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts;
    using Fanex.AccountingReports.Misc.Models;

    public interface ICreditSitesBetCountsService
    {
        Transaction GetReport(GetCreditSitesBetCountsCriteria criteria);

        void ExportReport(GetCreditSitesBetCountsCriteria criteria);
    }
}
