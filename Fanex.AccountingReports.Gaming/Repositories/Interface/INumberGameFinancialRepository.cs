// -----------------------------------------------------------------------
// <copyright file="INumberGameFinancialRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;

    public interface INumberGameFinancialRepository
    {
        DataSet GetNumberGameFinancial(GetNumberGameFinancialCriteria criteria);

        List<NumberGameFinancial> GetNumberGameFinancialDetail(DataSet ds);

        NumberGameFinancial GetLiveNumberPlayerAndGameReport(DataSet ds, NumberGameFinancial grandTotal, DateTime reportDate, bool isGrandDate, bool isLive);

        NumberGameFinancial GetGrandTotalNumberPlayerAndGameReport(DataSet ds, NumberGameFinancial grandTotal);
    }
}