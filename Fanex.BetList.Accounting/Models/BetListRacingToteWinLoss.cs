// -----------------------------------------------------------------------
// <copyright file="BetListRacingToteWinLoss.cs" company="Nexcel Solution Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListRacingToteWinLoss
    {
        public int No { get; set; }

        public string Username { get; set; }

        public string TransId { get; set; }

        public string TransDate { get; set; }

        public int BettypeId { get; set; }

        public string BettypeName { get; set; }

        public string WinOdds { get; set; }

        public string PlaceOdds { get; set; }

        public string Odds { get; set; }

        public string Stake { get; set; }

        public string MemberWinLoss { get; set; }

        public string PlayerComm { get; set; }

        public string AgencyPTWinLoss { get; set; }

        public string AgencyCommIncome { get; set; }

        public string IBCPTWinLoss { get; set; }

        public string IBCCommIncome { get; set; }

        public string WinStatus { get; set; }

        public string PlaceStatus { get; set; }

        public string Status { get; set; }

        public int HorseId { get; set; }

        public int HorseNo { get; set; }

        public string VirtualRate { get; set; }

        public bool IsShowOTExRate { get; set; }

        public string Dividend { get; set; }

        public string Choice { get; set; }

        public List<BetListRacingToteWinLoss> BetListDetail { get; set; }

        public BetListRacingToteWinLoss Total { get; set; }
    }
}
