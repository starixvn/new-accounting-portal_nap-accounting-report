using System.Collections.Generic;
using Fanex.BetList.Accounting.Criteria;
using Fanex.BetList.Accounting.Models;
using Fanex.Data.Repository;

namespace Fanex.BetList.Accounting.Services
{
    public interface IBetListService
    {
        IList<TicketDetail> GetBetListByCustInfo_Refno(string transactionId, int customerId);
        IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria criteria);
        BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria);
        IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria criteria);
        CombinationParlay GetCombinationMP(ICriteria criteria);
        IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria criteria, string handicap1, string handicap2, string betcheck, string betteam);
        BetListDetail GetWinLossMemberBetList(WinLossMemberBetListCriteria criteria);
    }
}