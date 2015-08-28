// --------------------------------------------------------------------------
// <copyright file="DirectBalanceDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Models.DirectBalance
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DirectBalanceDetail
    {
        public string CustId { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public string Balance { get; set; }

        public string BaseBalance { get; set; }
    }
}
