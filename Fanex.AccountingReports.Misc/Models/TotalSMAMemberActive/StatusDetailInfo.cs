// --------------------------------------------------------------------------
// <copyright file="StatusDetailInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using System.Web.Script.Serialization;
    using Fanex.AccountingReports.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class StatusDetailInfo
    {
        [ScriptIgnore]
        public DateTime Date { get; set; }

        public string Username { get; set; }

        public string Status { get; set; }

        public string Site { get; set; }

        public string Currency { get; set; }

        public string Position { get; set; }

        [ScriptIgnore]
        public DateTime CreatedTime { get; set; }

        public string Creator { get; set; }

        #region Display fields

        public string DateStr
        {
            get
            {
                return Date.ToShortFormatedDate();
            }
        }

        public string CreatedTimeStr
        {
            get
            {
                return CreatedTime.ToShortFormatedDateWithHourMinute();
            }
        }

        #endregion
    }
}