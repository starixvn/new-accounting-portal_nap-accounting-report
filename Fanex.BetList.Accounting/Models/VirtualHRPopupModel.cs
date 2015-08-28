// --------------------------------------------------------------------------
// <copyright file="VirtualHRPopupModel.cs" company="Nexcel Solutions Vietnam">
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
    public class VirtualHRPopupModel
    {
        public DateTime EventTime { get; set; }

        public int Place { get; set; }

        public int HorseNumber { get; set; }

        public string HorseName { get; set; }

        public decimal WinOdds { get; set; }

        public decimal PlaceOdds { get; set; }

        public string Status { get; set; }
    }
}