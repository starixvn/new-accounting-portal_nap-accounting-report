// -----------------------------------------------------------------------
// <copyright file="IKenoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Licensee.Criteria.KenoWinLoss;
    using Fanex.AccountingReports.Licensee.Models.KenoWinLoss;

    public interface IKenoWinLossService
    {
        List<SelectListItem> GetListCountryMarket();

        List<SelectListItem> GetListLicensees();

        List<SelectListItem> GetListCurrency(GetKenoCurrencyByLicenseeCriteria criteria);

        KenoWinLoss GetKenoWinLossReport(GetKenoWinLossCriteria criteria);

        GridOptions GetKenoWinLossReport2(GetKenoWinLossCriteria criteria);

        void ExportKenoWinLossReport(GetKenoWinLossCriteria criteria);
    }
}