// --------------------------------------------------------------------------
// <copyright file="NegativeBalanceModel.cs" company="Nexcel Solutions Vietnam">
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
    public class NegativeBalanceModel
    {
        public NegativeBalanceModel()
        {
            NegativeBalanceList = new List<NegativeBalance>();
            MailList = new List<NegativeBalanceMemberMailFormat>();
        }

        public List<NegativeBalance> NegativeBalanceList { get; set; }

        public List<NegativeBalanceMemberMailFormat> MailList { get; set; }

        public int TotalNegativeBalanceUser { get; set; }
    }
}
