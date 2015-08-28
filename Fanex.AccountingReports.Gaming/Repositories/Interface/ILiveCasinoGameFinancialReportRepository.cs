// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoGameFinancialReportRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;

    public interface ILiveCasinoGameFinancialReportRepository
    {
        List<EntityBase<int, string>> GetCurrencies();

        List<dynamic> GetGameTypes();

        List<dynamic> GetGameTables(GetGameTableCriteria criteria);

        List<dynamic> GetReseller(GetResellerCriteria criteria);

        List<dynamic> GetFinancialReport(GetFinancialReportCriteria criteria, ref Hashtable outputParam);
    }
}