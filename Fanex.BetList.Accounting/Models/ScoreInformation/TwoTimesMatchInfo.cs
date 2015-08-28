// --------------------------------------------------------------------------
// <copyright file="TwoTimesMatchInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class TwoTimesMatchInfo : ScoreInfo
    {
        public string HomeHTScore { get; set; }

        public string AwayHTScore { get; set; }

        public int HT1stScoreTeam { get; set; }

        public int Final1stScoreTeam { get; set; }

        /// <summary>
        /// Checks the cancellation.
        /// </summary>
        /// <param name="betStatus">The bet status.</param>
        public override void CheckCancellation(string betStatus)
        {
            var strNotAvailable = Labels.NotAvailable;
            var isCancelHT = betStatus.StartsWith(BetListConstants.ScoreInformationRefundHT) ||
                      betStatus.StartsWith(BetListConstants.ScoreInformationRefund);

            if (isCancelHT)
            {
                this.HomeHTScore = strNotAvailable;
                this.AwayHTScore = strNotAvailable;
            }

            base.CheckCancellation(betStatus);
        }
    }
}