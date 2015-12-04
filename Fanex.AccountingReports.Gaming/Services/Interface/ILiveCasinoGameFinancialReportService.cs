// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoGameFinancialReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.LiveCasinoGameFinancialReport
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameFinancialReport;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancialReport;

    public interface ILiveCasinoGameFinancialReportService
    {
        List<BaseModel> GetLicensees();

        List<BaseModel> GetCurrencies();

        List<ProductGameType> GetGameTypes(GetGameTypesCriteria criteria);

        List<BaseModel> GetGameTables(GetGameTableCriteria criteria);

        FinancialReport GetFinancialReport(GetFinancialReportCriteria criteria);

        void ExportFinancialReport(GetFinancialReportCriteria criteria);
    }
}