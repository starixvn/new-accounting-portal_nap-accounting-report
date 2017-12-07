namespace Fanex.BetList.Accounting.Models
{
    using System;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ScoreInfo
    {
        public string TraderName { get; set; }

        public string EventTime { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; }

        public string EventStatus { get; set; }

        public bool IsNeutral { get; set; }

        public bool IsPending { get; set; }

        public string HomeFinalScore { get; set; }

        public string AwayFinalScore { get; set; }

        /// <summary>
        /// Checks the cancellation.
        /// </summary>
        /// <param name="betStatus">The bet status.</param>
        public virtual void CheckCancellation(string betStatus)
        {
            var strNotAvailable = Labels.NotAvailable;
            var isCancelFinal = betStatus.StartsWith(BetListConstants.ScoreInformationRefundFT) ||
              betStatus.StartsWith(BetListConstants.ScoreInformationRefund);

            if (isCancelFinal)
            {
                this.HomeFinalScore = strNotAvailable;
                this.AwayFinalScore = strNotAvailable;
            }
        }
    }
}
