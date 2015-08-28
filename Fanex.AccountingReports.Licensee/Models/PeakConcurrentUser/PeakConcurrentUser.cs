// --------------------------------------------------------------------------
// <copyright file="PeakConcurrentUser.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class PeakConcurrentUser
    {
        public PeakConcurrentUser()
        {
            ListConcurrentUser = new List<ConcurrentUser>();
        }

        public List<ConcurrentUser> ListConcurrentUser { get; set; }

        public string AverageUsers { get; set; }

        public string Total { get; set; }

        public string MaxUsers { get; set; }

        public string MinUsers { get; set; }
    }
}
