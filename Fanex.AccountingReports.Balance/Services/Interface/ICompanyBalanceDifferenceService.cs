// --------------------------------------------------------------------------
// <copyright file="ICompanyBalanceDifferenceService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Models.CompanyBalanceDifference;

    public interface ICompanyBalanceDifferenceService
    {
        List<DifferenceInfo> GetCompanyBalanceDifference();
    }
}
