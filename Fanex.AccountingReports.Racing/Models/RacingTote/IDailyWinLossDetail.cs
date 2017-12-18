namespace Fanex.AccountingReports.Racing.Models.RacingTote
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDailyWinLossDetail
    {
        int RaceNo { get; set; }

        string LeagueName { get; set; }

        int MeetingId { get; set; }

        string CurrencyName { get; set; }

        int CurrencyId { get; set; }

        decimal WinTOAgency { get; set; }

        decimal PlaceTOAgency { get; set; }

        decimal WinPlaceTOAgency { get; set; }

        decimal TotalTOAgency { get; set; }

        decimal CommIncomeAgency { get; set; }

        decimal PTWinLossAgency { get; set; }

        decimal WinTOIBC { get; set; }

        decimal PlaceTOIBC { get; set; }

        decimal WinPlaceTOIBC { get; set; }

        decimal TotalTOIBC { get; set; }

        decimal CommIncomeIBC { get; set; }

        decimal ToteFeeIBC { get; set; }

        decimal CommExpenseIBC { get; set; }

        decimal PTWinLossIBC { get; set; }

        decimal NetIncomeIBC { get; set; }

        decimal WinTOPartner { get; set; }

        decimal PlaceTOPartner { get; set; }

        decimal WinPlaceTOPartner { get; set; }

        decimal TotalTOPartner { get; set; }

        decimal DividendWonPartner { get; set; }

        decimal CommExpensePartner { get; set; }

        decimal ToteFeePartner { get; set; }

        decimal AmountDuePartner { get; set; }

        List<WinLossToteDetail> ChildsInfo { get; set; }

        WinLossToteDetail Total { get; set; }
    }
}