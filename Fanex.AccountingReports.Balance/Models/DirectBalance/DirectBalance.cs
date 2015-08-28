// --------------------------------------------------------------------------
// <copyright file="DirectBalance.cs" company="Nexcel Solutions Vietnam">
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
    public class DirectBalance
    {
        public List<DirectBalanceDetail> DirectBalanceList { get; set; }

        public string BaseBalanceTotal { get; set; }
    }
}
