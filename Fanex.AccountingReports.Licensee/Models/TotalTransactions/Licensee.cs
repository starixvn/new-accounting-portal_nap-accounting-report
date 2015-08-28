// --------------------------------------------------------------------------
// <copyright file="Licensee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.TotalTransactions
{
    using System;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class Licensee : EntityBase<int, string>
    {
        public Reseller Reseller { get; set; }
    }
}