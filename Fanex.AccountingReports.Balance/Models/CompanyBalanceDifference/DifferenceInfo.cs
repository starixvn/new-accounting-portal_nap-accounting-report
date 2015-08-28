// --------------------------------------------------------------------------
// <copyright file="DifferenceInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Models.CompanyBalanceDifference
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
     public class DifferenceInfo
    {
        public string BalanceUpDate { get; set; }

        public decimal BalanceBeforeBalanceUpDate { get; set; }

        public decimal BalanceAfterBalanceUpDate { get; set; }

        public decimal Difference { get; set; }
    }
}
