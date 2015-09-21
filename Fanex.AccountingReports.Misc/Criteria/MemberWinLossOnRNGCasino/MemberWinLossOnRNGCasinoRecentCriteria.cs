namespace Fanex.AccountingReports.Misc.Criteria.MemberWinLossOnRNGCasino
{
    using System;
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberWinLossOnRNGCasinoRecentCriteria : IMemberWinLossOnRNGCasinoCriteria
    {
        public double Amount { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsAutoRefresh { get; set; }

        public bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }

        public string GetSettingKey()
        {
            return "Misc_MemberWinLossOnRNGCasinoRecent";
        }

        public Hashtable GetSpParams()
        {
            Hashtable p = new Hashtable();
            p.Add("amount", Amount);
            p.Add("fromdate", FromDate);
            p.Add("todate", ToDate);
            
            return p;
        }
    }
}
