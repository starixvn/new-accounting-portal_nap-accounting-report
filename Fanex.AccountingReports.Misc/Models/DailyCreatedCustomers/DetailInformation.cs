// --------------------------------------------------------------------------
// <copyright file="DetailInformation.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DetailInformation
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string CreatedDate { get; set; }

        public string Creator { get; set; }
    }
}
