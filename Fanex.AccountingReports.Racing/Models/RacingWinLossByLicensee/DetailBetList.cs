namespace Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee
{
    using System;

    /// <summary>
    /// The model of detail bet list of report Racing Win Loss By Licensee.
    /// </summary>
    [Serializable]
    public class DetailBetList
    {
        public DetailBetList()
        {
            Member = string.Empty;
            Username = string.Empty;
            TransactionDate = string.Empty;
            TransactionID = string.Empty;
            HorseBetType = string.Empty;
            BetType = string.Empty;
            Status = string.Empty;
            WinStatus = string.Empty;
            PlaceStatus = string.Empty;
        }

        public string Member { get; set; }

        public string Username { get; set; }

        public string TransactionID { get; set; }

        public string TransactionDate { get; set; }

        public string HorseBetType { get; set; }

        public string BetType { get; set; }

        public bool IsShowExchangeRate { get; set; }

        public bool IsCombineBetType { get; set; }

        public decimal Odds { get; set; }

        public decimal WinOdds { get; set; }

        public decimal PlaceOdds { get; set; }

        public decimal Stake { get; set; }

        public decimal ActualRate { get; set; }

        public string ExchangeRate { get; set; } 

        public decimal MemberWinLoss { get; set; }

        public decimal MemberCommissionIncome { get; set; }

        public decimal LicenseeWinLossPositionTaking { get; set; }

        public decimal LicenseeCommissionIncome { get; set; }

        public decimal IBCWinLossPositionTaking { get; set; }

        public decimal IBCCommissionIncome { get; set; }

        public string Status { get; set; }

        public string WinStatus { get; set; }

        public string PlaceStatus { get; set; }

        public decimal Dividend { get; set; }

        public string Odds_Str { get; set; }

        public void SumTotal(DetailBetList bet)
        {
            Stake += bet.Stake;
            MemberWinLoss += bet.MemberWinLoss;
            MemberCommissionIncome += bet.MemberCommissionIncome;
            LicenseeWinLossPositionTaking += bet.LicenseeWinLossPositionTaking;
            LicenseeCommissionIncome += bet.LicenseeCommissionIncome;
            IBCWinLossPositionTaking += bet.IBCWinLossPositionTaking;
            IBCCommissionIncome += bet.IBCCommissionIncome;
            Dividend += bet.Dividend;
        }
    }
}
