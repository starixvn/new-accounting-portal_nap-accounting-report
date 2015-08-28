// --------------------------------------------------------------------------
// <copyright file="SMASummaryInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SMASummaryInfo
    {
        /// <summary>
        /// Gets or sets status of SMA: New Created/Active/Suspended/Closed.
        /// </summary>
        /// <value>
        /// The Status of SMA.
        /// </value>
        public EntityBase<int, string> Key { get; set; }

        public int Super { get; set; }

        public int Master { get; set; }

        public int Agent { get; set; }
    }
}
