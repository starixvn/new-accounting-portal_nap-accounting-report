// --------------------------------------------------------------------------
// <copyright file="NegativeBalance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalance
    {
        public NegativeBalance()
        {
            NegativeBalanceUserList = new List<NegativeBalanceUser>();
        }

        public string LicenseeSite { get; set; }

        public int TotalNegativeBalanceList { get; set; }

        public List<NegativeBalanceUser> NegativeBalanceUserList { get; set; }
    }
}
