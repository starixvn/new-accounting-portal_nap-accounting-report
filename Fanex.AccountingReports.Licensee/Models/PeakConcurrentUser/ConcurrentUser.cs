// --------------------------------------------------------------------------
// <copyright file="ConcurrentUser.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ConcurrentUser
    {
        public string Date { get; set; }

        public string TotalUser { get; set; }
    }
}
