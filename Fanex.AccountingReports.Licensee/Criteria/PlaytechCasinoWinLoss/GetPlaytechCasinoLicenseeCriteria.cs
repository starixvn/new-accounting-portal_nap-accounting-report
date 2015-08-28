// -----------------------------------------------------------------------
// <copyright file="GetPlaytechCasinoLicenseeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get play tech casino win loss criteria.
    /// </summary>
    public class GetPlaytechCasinoLicenseeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_PlaytechCS_LicenseeSelAll";
        }
    }
}