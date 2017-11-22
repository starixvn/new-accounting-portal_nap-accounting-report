namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class VirtualRateInfo
    {
        public VirtualRateInfo()
        {
            ReportRows = new List<VirtualRateInfo>();
            VirtualRateByBettype = new List<VirtualRateByBettypeInfo>();
        }

        public bool EnableVirtualRate { get; set; }

        public string Date { get; set; }

        public string Sport { get; set; }

        public string LiveStatus { get; set; }

        public List<VirtualRateByBettypeInfo> VirtualRateByBettype { get; set; }

        public string ChangedBy { get; set; }

        public int NumberMergeRows { get; set; }

        public List<VirtualRateInfo> ReportRows { get; set; }

        public void CalculateNumberMergeRows()
        {
            int numberMergeRows = 0;
            foreach (var row in this.ReportRows)
            {
                numberMergeRows += row.VirtualRateByBettype.Count;
            }

            this.NumberMergeRows = numberMergeRows;
        }
    }
}
