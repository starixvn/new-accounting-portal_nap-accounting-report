namespace Fanex.AccountingReports.Racing.Models.RacingTote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Create Win Loss Tote Detail model.
    /// </summary>
    [Serializable]
    public class WinLossToteDetail : IDailyWinLossDetail
    {
        public WinLossToteDetail()
        {
            ChildsInfo = new List<WinLossToteDetail>();
        }

        public int RaceNo { get; set; }

        public string LeagueName { get; set; } 

        public int MeetingId { get; set; }

        public string CurrencyName { get; set; }

        public int CurrencyId { get; set; }

        public decimal WinTOAgency { get; set; }

        public decimal PlaceTOAgency { get; set; }

        public decimal WinPlaceTOAgency { get; set; }

        public decimal TotalTOAgency { get; set; }

        public decimal CommIncomeAgency { get; set; }

        public decimal PTWinLossAgency { get; set; }

        public decimal WinTOIBC { get; set; }

        public decimal PlaceTOIBC { get; set; }

        public decimal WinPlaceTOIBC { get; set; }

        public decimal TotalTOIBC { get; set; }

        public decimal CommIncomeIBC { get; set; }

        public decimal ToteFeeIBC { get; set; }

        public decimal CommExpenseIBC { get; set; }

        public decimal PTWinLossIBC { get; set; }

        public decimal NetIncomeIBC { get; set; }

        public decimal WinTOPartner { get; set; }

        public decimal PlaceTOPartner { get; set; }

        public decimal WinPlaceTOPartner { get; set; }

        public decimal TotalTOPartner { get; set; }

        public decimal DividendWonPartner { get; set; }

        public decimal CommExpensePartner { get; set; }

        public decimal ToteFeePartner { get; set; }

        public decimal AmountDuePartner { get; set; }

        public List<WinLossToteDetail> ChildsInfo { get; set; }

        public WinLossToteDetail Total { get; set; }

        /// <summary>
        /// Sum win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void SumRacingWinLossDetail(WinLossToteDetail winLoss)
        {
            if (winLoss != null)
            {
                WinTOAgency += winLoss.WinTOAgency;
                PlaceTOAgency += winLoss.PlaceTOAgency;
                WinPlaceTOAgency += winLoss.WinPlaceTOAgency;
                TotalTOAgency += winLoss.TotalTOAgency;
                CommIncomeAgency += winLoss.CommIncomeAgency;
                PTWinLossAgency += winLoss.PTWinLossAgency;
                WinTOIBC += winLoss.WinTOIBC;
                PlaceTOIBC += winLoss.PlaceTOIBC;
                WinPlaceTOIBC += winLoss.WinPlaceTOIBC;
                TotalTOIBC += winLoss.TotalTOIBC;
                CommIncomeIBC += winLoss.CommIncomeIBC;
                ToteFeeIBC += winLoss.ToteFeeIBC;
                CommExpenseIBC += winLoss.CommExpenseIBC;
                PTWinLossIBC += winLoss.PTWinLossIBC;
                NetIncomeIBC += winLoss.NetIncomeIBC;
                WinTOPartner += winLoss.WinTOPartner;
                PlaceTOPartner += winLoss.PlaceTOPartner;
                WinPlaceTOPartner += winLoss.WinPlaceTOPartner;
                TotalTOPartner += winLoss.TotalTOPartner;
                DividendWonPartner += winLoss.DividendWonPartner;
                CommExpensePartner += winLoss.CommExpensePartner;
                ToteFeePartner += winLoss.ToteFeePartner;
                AmountDuePartner += winLoss.AmountDuePartner;
            }
        }
    }
}
