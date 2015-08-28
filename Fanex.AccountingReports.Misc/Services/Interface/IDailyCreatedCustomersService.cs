// --------------------------------------------------------------------------
// <copyright file="IDailyCreatedCustomersService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.DailyCreatedCustomers;
    using Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers;

    public interface IDailyCreatedCustomersService
    {
        SummaryInformation GetReport(GetDailyCreatedCustomersCriteria criteria);

        List<DetailInformation> GetReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria);

        void ExportReport(GetDailyCreatedCustomersCriteria criteria);

        void ExportReportDetails(GetDailyCreatedCustomersDetailsCriteria criteria);
    }
}
