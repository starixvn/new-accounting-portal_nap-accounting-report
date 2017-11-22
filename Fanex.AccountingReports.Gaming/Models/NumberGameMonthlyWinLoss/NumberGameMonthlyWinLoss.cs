namespace Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Number Game Monthly Win Loss Model.
    /// </summary>
    [Serializable]
    public class NumberGameMonthlyWinLoss
    {
        public NumberGameMonthlyWinLoss()
        {
            ReportRows = new List<NumberGameMonthlyWinLoss>();
        }

        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Winloss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal Linetotal { get; set; }

        public decimal Turnover { get; set; }

        public string Margin { get; set; }

        public List<NumberGameMonthlyWinLoss> ReportRows { get; set; }

        /// <summary>
        /// Get total value of report.
        /// </summary>
        /// <param name="reportInfo">The report information.</param>
        public void CalculateTotal(NumberGameMonthlyWinLoss reportInfo)
        {
            if (reportInfo == null)
            {
                return;
            }

            this.Winloss += reportInfo.Winloss;
            this.Position += reportInfo.Position;
            this.Commission += reportInfo.Commission;
            this.Linetotal += reportInfo.Linetotal;
            this.Turnover += reportInfo.Turnover;
        }
    }
}
