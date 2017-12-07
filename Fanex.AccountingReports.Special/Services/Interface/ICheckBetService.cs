namespace Fanex.AccountingReports.Special.Services.Interface
{
    using Fanex.AccountingReports.Special.Criteria.CheckBet;
    using Fanex.AccountingReports.Special.Models;

    public interface ICheckBetService
    {
        CheckBet GetCheckedBet(CheckBetCriteria checkBetCriteria);

        NormalTickets GetNormalChangedTicket(CheckBetCriteria checkBetCriteria);

        RejectTickets GetRejectChangedTicket(CheckBetCriteria checkBetCriteria);
    }
}
