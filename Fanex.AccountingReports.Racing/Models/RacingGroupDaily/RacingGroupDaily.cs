namespace Fanex.AccountingReports.Racing.Models.RacingGroupDaily
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The model of Racing Group Daily report.
    /// </summary>
    [Serializable]
    public class RacingGroupDaily
    {
        public RacingGroupDaily()
        {
            WinLostDate = string.Empty;
            Name = string.Empty;
            Rows = new List<RacingGroupDaily>();
        }

        public string WinLostDate { get; set; }

        public string Name { get; set; }

        public decimal TurnoverAgent { get; set; }

        public decimal TurnoverIBC { get; set; }

        public decimal TotalTurnover { get; set; }

        public decimal IncomeIBCPostitionTaking { get; set; }

        public decimal IncomeIBCNetCommission { get; set; }

        public decimal TotalIBCIncome { get; set; }

        public decimal Income12BETPositionTaking { get; set; }

        public decimal Income12BETCommission { get; set; }

        public decimal Total12BETIncome { get; set; }

        public decimal GroupTotalIncome { get; set; }

        public decimal PartnerTurnover { get; set; }

        public decimal PartnerDividendWon { get; set; }

        public decimal PartnerExpenseCommission { get; set; }

        public decimal PartnerFee { get; set; }

        public decimal PartnerAmountDue { get; set; }

        public List<RacingGroupDaily> Rows { get; set; }

        /// <summary>
        /// Calculate the sum total.
        /// </summary>
        /// <param name="racingGroupDaily">The racing group daily.</param>
        public void SumTotal(RacingGroupDaily racingGroupDaily)
        {
            this.TurnoverAgent += racingGroupDaily.TurnoverAgent;
            this.TurnoverIBC += racingGroupDaily.TurnoverIBC;
            this.TotalTurnover += racingGroupDaily.TotalTurnover;
            this.IncomeIBCPostitionTaking += racingGroupDaily.IncomeIBCPostitionTaking;
            this.IncomeIBCNetCommission += racingGroupDaily.IncomeIBCNetCommission;
            this.TotalIBCIncome += racingGroupDaily.TotalIBCIncome;
            this.Income12BETPositionTaking += racingGroupDaily.Income12BETPositionTaking;
            this.Income12BETCommission += racingGroupDaily.Income12BETCommission;
            this.Total12BETIncome += racingGroupDaily.Total12BETIncome;
            this.GroupTotalIncome += racingGroupDaily.GroupTotalIncome;
            this.PartnerTurnover += racingGroupDaily.PartnerTurnover;
            this.PartnerDividendWon += racingGroupDaily.PartnerDividendWon;
            this.PartnerExpenseCommission += racingGroupDaily.PartnerExpenseCommission;
            this.PartnerFee += racingGroupDaily.PartnerFee;
            this.PartnerAmountDue += racingGroupDaily.PartnerAmountDue;
        }
    }
}
