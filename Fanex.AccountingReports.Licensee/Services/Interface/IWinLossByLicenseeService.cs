// --------------------------------------------------------------------------
// <copyright file="IWinLossByLicenseeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Models.WinLossByLicensee;

    public interface IWinLossByLicenseeService
    {
        WinLossByLicensee GetCurrencyReport(GetCurrencyReportCriteria criteria);

        WinLossByLicensee GetLevelReport(GetLevelReportByCustIdCriteria criteria);

        void ExportCurrencyReport(GetCurrencyReportCriteria criteria);

        void ExportLevelReport(GetLevelReportByCustIdCriteria criteria);
    }
}
