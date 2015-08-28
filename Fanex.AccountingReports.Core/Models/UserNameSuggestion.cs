// --------------------------------------------------------------------------
// <copyright file="UserNameSuggestion.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserNameSuggestion
    {
        public string QueryValue { get; set; }

        // JQuery Plugin user suggestions field name to get data back
        public List<string> Suggestions { get; set; }

        public string[] DataInfo { get; set; }
    }
}
