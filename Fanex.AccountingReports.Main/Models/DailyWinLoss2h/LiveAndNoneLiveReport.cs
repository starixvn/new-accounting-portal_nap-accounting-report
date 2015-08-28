// --------------------------------------------------------------------------
// <copyright file="LiveAndNoneLiveReport.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
   public class LiveAndNoneLiveReport
    {
        public WinLossInfo NoneLiveReport { get; set; }

        public WinLossInfo LiveReport { get; set; }

        public WinLossInfo MixLiveAndNoneLiveReport { get; set; }

        public WinLossInfo GrandTotal { get; set; }
    }
}
