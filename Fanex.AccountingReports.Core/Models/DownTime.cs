// --------------------------------------------------------------------------
// <copyright file="Downtime.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.9">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DownTime
    {
        public DownTime()
        {
            From = "0:00";
            To = "0:00";
            IsDowntime = false;
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool IsDowntime { get; set; }
    }
}