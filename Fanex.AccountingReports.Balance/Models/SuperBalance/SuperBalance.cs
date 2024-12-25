namespace Fanex.AccountingReports.Balance.Models.SuperBalance
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SuperBalance
    {
        public string CustId { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public bool HeadlineEnabled { get; set; }

        public decimal Balance { get; set; }

        public decimal BaseBalance { get; set; }

        public decimal Reward { get; set; }

        public decimal RewardRM { get; set; }

        public bool HasData { get; set; }

        public List<SuperBalance> SuperBalanceList { get; set; }

        public SuperBalance BaseBalanceTotal { get; set; }

        public static SuperBalance FormatInfo(SuperBalance balanceInfo)
        {
            if (balanceInfo == null)
            {
                return balanceInfo;
            }

            balanceInfo.Balance = Math.Round(balanceInfo.Balance, 2, MidpointRounding.AwayFromZero);
            balanceInfo.BaseBalance = Math.Round(balanceInfo.BaseBalance, 2, MidpointRounding.AwayFromZero);
            return balanceInfo;
        }

        public void AddToBaseBalance(SuperBalance superBalance)
        {
            if (superBalance == null)
            {
                return;
            }

            this.BaseBalance += superBalance.BaseBalance;
            this.RewardRM += superBalance.RewardRM;
        }
    }
}
