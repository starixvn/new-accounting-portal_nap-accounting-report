// --------------------------------------------------------------------------
// <copyright file="VirtualTennisModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualTennisModel
    {
        public DateTime KickOffTime { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public int HomePoints { get; set; }

        public int AwayPoints { get; set; }

        public string EventStatus { get; set; }

        public string LeageName { get; set; }
    }
}