// --------------------------------------------------------------------------
// <copyright file="InputedLegend.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Models
{
    /// <summary>
    /// Inputted Legend.
    /// </summary>
    public class InputedLegend
    {
        public string System { get; set; }

        public int ReportId { get; set; }

        public string ReportName { get; set; }

        public string Content { get; set; }
    }
}