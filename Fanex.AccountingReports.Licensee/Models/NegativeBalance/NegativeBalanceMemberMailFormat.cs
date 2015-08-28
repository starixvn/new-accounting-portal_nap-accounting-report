// --------------------------------------------------------------------------
// <copyright file="NegativeBalanceMemberMailFormat.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalanceMemberMailFormat
    {
        public string Site { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }
    }
}
