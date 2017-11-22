using Fanex.BetList.Accounting.Extensions;
using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Accounting.Repositories;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Factories;
using Fanex.BetList.Core.TicketHelper;
using Fanex.Data.Repository;

namespace Fanex.BetList.Accounting.Services
{
    public interface BetListCashoutDetailService
    {
        CashOutHistory GetCashOutHistory(ICriteria criteria);
    }

    public class BetListCashoutDetailServiceImp : BetListCashoutDetailService
    {
        private readonly BetListCashoutDetailRepository _betListDetailRepository;

        public BetListCashoutDetailServiceImp(BetListCashoutDetailRepository betListDetailRepository = null)
        {
            _betListDetailRepository = betListDetailRepository ?? new BetListCashoutDetailRepositoryImp();
        }

        public CashOutHistory GetCashOutHistory(ICriteria criteria)
        {
            var cashOutHistory = _betListDetailRepository.GetCashOutHistory(criteria);
            var ticketDetail = cashOutHistory.TicketDetail;

            if (ticketDetail != null)
            {
                ITicketHelper ticketHelper = new TicketHelper();
                ticketHelper.HideStatusCashOutHistory = true;

                var factory = new BetListHTMLFactory();
                var builder = factory.CreateBuilder();
                var ticket = ticketDetail.Ticket;

                ticketDetail.TransDateHtml = builder.BuildTrans(ticketDetail.Ticket);
                ticketDetail.ChoiceHtml = builder.BuildChoice(ticket, ticketHelper, null, false);
                ticketDetail.OddHtml = builder.BuildOdds(ticket, null, CacheRef.GetOddsTypeName);
                ticketDetail.StakeHtml = BetListExtension.BuildStake(ticketDetail, ref builder);

                var statusBuilder = builder.CreateStatusBuilder(ticket.BetTypeId);
                var statusBlock = statusBuilder.Render(ticket, ticketHelper, null, true);
                statusBlock.ShowIP = null;
                statusBlock.StatusResult = null;
                ticketDetail.StatusHtml = statusBlock.ToString();

                if (cashOutHistory.CashOutTickets != null)
                {
                    foreach (var cashoutTicket in cashOutHistory.CashOutTickets)
                    {
                        statusBuilder = builder.CreateStatusBuilder(cashoutTicket.BetType);
                        statusBlock = statusBuilder.Render(ticket, ticketHelper, null, true);
                        statusBlock.ShowIP = null;
                        statusBlock.StatusResult = null;
                        cashoutTicket.StatusHtml = statusBlock.ToString();
                    }
                }
            }

            return cashOutHistory;
        }
    }
}

