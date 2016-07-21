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
    using Fanex.AccountingReports.Misc.Criteria.BetCount;
    using Fanex.AccountingReports.Misc.Models;
    using Models.BetCount;
    using System.Collections.Generic;

    public interface IBetCountService
    {
        Transaction GetReport(GetBetCountReportCriteria criteria);

        Transaction SortReport(GetBetCountReportCriteria criteria);

        void ExportReport(GetBetCountReportCriteria criteria);

        List<ProductInfo> GetSports(GetProductsCriteria criteria);

        List<ProductInfo> GetProducts(GetProductsCriteria crteria);
    }
}
