// -----------------------------------------------------------------------
// <copyright file="INumberGameFinancialExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;

    public interface INumberGameFinancialExportDataService
    {
        void ExportNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria);
    }
}