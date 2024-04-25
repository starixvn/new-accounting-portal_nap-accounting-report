using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
	public class BettypeWinLossWithChoiceModel
	{
		public List<BetTypeWinLossItemByChoice> BettypeWinLoss { get; set; }

		public BettypeWinLossBySite Total { get; set; }

		public BettypeWinLossBySite RM { get; set; }

		public BettypeWinLossBySite BARM { get; set; }

		public BettypeWinLossBySite GrandTotal { get; set; }
	}
}
