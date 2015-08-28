// -----------------------------------------------------------------------
// <copyright file="NumberGameMonthlyWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.10.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Number Game Monthly Win Loss Criteria.
    /// </summary>
    public class NumberGameMonthlyWinLossCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameMonthlyWinloss";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}