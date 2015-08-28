// -----------------------------------------------------------------------
// <copyright file="GetKenoWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.KenoWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get play tech casino win loss criteria.
    /// </summary>
    [Serializable]
    public class GetKenoWinLossCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LicenseeIDList")]
        public string LicenseeID { get; set; }

        [SpParam(SqlParamName = "bettypeID")]
        public string CountryMarket { get; set; }

        public string Currency { get; set; }

        [SpParam(Ignored = true)]
        public string LicenseeFileNameString { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_KenoWinLoss_Report";
        }
    }
}