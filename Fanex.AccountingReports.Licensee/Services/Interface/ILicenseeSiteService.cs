// --------------------------------------------------------------------------
// <copyright file="ILicenseeSiteService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILicenseeSiteService
    {
        List<Licensee> GetLicenseeSiteList();

        List<Currency> GetCurrenciesByLicensee(GetCurrenciesByLicenseeCriteria criteria);

        List<Licensee> GetPlaytechCasino();
    }
}
