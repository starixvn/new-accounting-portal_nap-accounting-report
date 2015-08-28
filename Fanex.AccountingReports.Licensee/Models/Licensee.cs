// --------------------------------------------------------------------------
// <copyright file="Licensee.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class Licensee
    {
        public int ResellerId { get; set; }

        public string ResellerName { get; set; }

        public int ResellerType { get; set; }

        public int LicenseeId { get; set; }

        public string LicenseeName { get; set; }

        public bool IsEnabledBA { get; set; }

        public string BrandName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [is multi agent].
        /// Explanation: 12Bet has multi agent such as: affiliate, banner, ... for business.
        /// It's different from others (dummy super/master/agent).
        /// </summary>
        /// <value>
        ///   <c>true</c> If [is multi agent]; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiAgent { get; set; }
    }
}
