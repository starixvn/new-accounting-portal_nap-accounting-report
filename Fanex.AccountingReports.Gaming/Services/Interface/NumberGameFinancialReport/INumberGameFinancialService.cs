// -----------------------------------------------------------------------
// <copyright file="INumberGameFinancialService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;

    public interface INumberGameFinancialService
    {
        NumberGameFinancial GetNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria);
    }
}