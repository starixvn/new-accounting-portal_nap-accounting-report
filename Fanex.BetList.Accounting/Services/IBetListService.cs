using Fanex.BetList.Accounting.Criteria;
using Fanex.BetList.Accounting.Models;
using Fanex.Data.Repository;
using System.Collections.Generic;

namespace Fanex.BetList.Accounting.Services
{
    public interface IBetListService
    {
        IList<TicketDetail> GetBetListByRefNoTickets(BetListByRefNoCriteria criteria);
        BetListRacingToteWinLoss GetBetListRacingToteWinLoss(BetListRacingWinLossToteCriteria criteria);
        IList<BetType> GetBetTypeByDate(BetTypeByDateCriteria criteria);
        CombinationParlay GetCombinationMP(ICriteria criteria);
        IList<TicketDetail> GetMixParlayTickets(MixParlayDetailCriteria criteria, string handicap1, string handicap2, string betcheck, string betteam);
    }
}