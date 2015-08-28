// -----------------------------------------------------------------------
// <copyright file="IReportingService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.11">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Interface
{
    using Fanex.AccountingReports.Special.Criteria.ReportingServices;
    using Fanex.AccountingReports.Special.Models.ReportingServices;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IReportingService
    {
        Report GetReport(GetReportCriteria criteria);

        string GetMimeType(string fileExtension);
    }
}