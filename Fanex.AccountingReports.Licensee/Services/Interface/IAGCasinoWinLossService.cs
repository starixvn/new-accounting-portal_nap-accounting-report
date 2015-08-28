// --------------------------------------------------------------------------
// <copyright file="IAGCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Models.AGCasinoWinloss;

    public interface IAGCasinoWinLossService
    {
        List<Licensee> Licensee();

        List<SelectListItem> CurrencyByLicensee(GetCurrencyByLicensee criteria);

        AGCasinoWinlossData GetAGCasinoWinlossMainReport(GetMainReportCriteria criteria);

        void ExportAGCasinoWinlossMainReport(GetMainReportCriteria criteria);

        AGCasinoWinlossData GetAGCasinoWinlossSMAMReport(GetMainReportCriteria criteria);

        void ExportAGCasinoWinlossSMAMReport(GetMainReportCriteria criteria);
    }
}
