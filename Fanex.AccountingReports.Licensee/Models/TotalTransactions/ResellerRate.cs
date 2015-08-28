// --------------------------------------------------------------------------
// <copyright file="ResellerRate.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.TotalTransactions
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ResellerRate
    {
        public int ResellerID { get; set; }

        public string ResellerName { get; set; }

        public int MinTrans { get; set; }

        public int MaxTrans { get; set; }

        public decimal Rate { get; set; }
    }
}
