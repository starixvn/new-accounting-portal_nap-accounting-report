// --------------------------------------------------------------------------
// <copyright file="WinLossByLicenseeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using System;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossBySuperCriteria : WinLossByLicenseeCriteria
    {
        [SpParam(Ignored = true)]
        public int CustID { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_WinLossByLicensee_GetWinLossSuper";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}