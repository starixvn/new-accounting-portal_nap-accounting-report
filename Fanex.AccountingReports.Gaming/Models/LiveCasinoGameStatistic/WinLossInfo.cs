namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class WinLossInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int NumberOfPlayers { get; set; }

        public int NumberOfGames { get; set; }

        public string CurrencyName { get; set; }

        public decimal TotalBets { get; set; }

        public decimal TotalWins { get; set; }

        public decimal TotalRefund { get; set; }

        public decimal GameIncome { get; set; }

        public decimal GamePayout
        {
            get
            {
                return Math.Round(TotalBets == 0 ? 0 : TotalWins / TotalBets, 4);
            }
        }

        public decimal CompanyTurnover { get; set; }

        public decimal CompanyNetIncome { get; set; }

        public decimal CompanyCommission { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.TotalBets = Math.Round(winLoss.TotalBets, 2);
            winLoss.TotalWins = Math.Round(winLoss.TotalWins, 2);
            winLoss.TotalRefund = Math.Round(winLoss.TotalRefund, 2);

            winLoss.GameIncome = Math.Round(winLoss.GameIncome, 2);

            winLoss.CompanyTurnover = Math.Round(winLoss.CompanyTurnover, 2);
            winLoss.CompanyNetIncome = Math.Round(winLoss.CompanyNetIncome, 2);
            winLoss.CompanyCommission = Math.Round(winLoss.CompanyCommission, 2);
            return winLoss;
        }

        public void Sum(WinLossInfo winLoss)
        {
            this.NumberOfPlayers += winLoss.NumberOfPlayers;
            this.NumberOfGames += winLoss.NumberOfGames;

            this.TotalBets += winLoss.TotalBets;
            this.TotalWins += winLoss.TotalWins;
            this.TotalRefund += winLoss.TotalRefund;

            this.GameIncome += winLoss.GameIncome;

            this.CompanyTurnover += winLoss.CompanyTurnover;
            this.CompanyNetIncome += winLoss.CompanyNetIncome;
            this.CompanyCommission += winLoss.CompanyCommission;
        }
    }
}
