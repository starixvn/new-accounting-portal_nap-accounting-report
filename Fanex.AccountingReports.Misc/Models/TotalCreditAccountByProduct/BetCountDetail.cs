namespace Fanex.AccountingReports.Misc.Models.TotalCreditAccountByProduct
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class BetCountDetail
    {
        public BetCountDetail()
        {
            ReportRows = new List<BetCountDetail>();
        }

        public string UserName { get; set; }

        public bool IsDisable { get; set; }

        public int SettledBetCount { get; set; }

        public int RunningBetCount { get; set; }

        public List<BetCountDetail> ReportRows { get; set; }

        public BetCountDetail Total { get; set; }

        public int ReportLength { get; set; }

        public void CalculateTotal(BetCountDetail betCountDetail)
        {
            this.SettledBetCount += betCountDetail.SettledBetCount;
            this.RunningBetCount += betCountDetail.RunningBetCount;
        }
    }
}
