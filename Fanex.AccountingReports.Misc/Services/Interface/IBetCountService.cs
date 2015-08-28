// --------------------------------------------------------------------------
// <copyright file="IBetCountService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Misc.Criteria.BetCount;
    using Fanex.AccountingReports.Misc.Models;

    public interface IBetCountService
    {
        List<EntityBase<int, string>> GetAllProducts();

        Transaction GetReport(GetBetCountReportCriteria criteria);

        Transaction SortReport(GetBetCountReportCriteria criteria);

        void ExportReport(GetBetCountReportCriteria criteria);
    }
}
