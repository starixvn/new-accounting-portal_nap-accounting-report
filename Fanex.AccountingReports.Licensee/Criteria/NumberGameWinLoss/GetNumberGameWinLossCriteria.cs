// --------------------------------------------------------------------------
// <copyright file="GetNumberGameWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.NumberGameWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetNumberGameWinLossCriteria : CriteriaBase, ICriteria
    {
        public string NormalLicensee { get; set; }

        public string Operator { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_NumberGameWinloss_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
