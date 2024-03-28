using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
	[Serializable]
	public class BetTypeWinLossItemByChoice
	{
		public BettypeWinLossBySite Total { get; set; }

		public List<BetTypeWinLossChoiceItem> BetTypes { get; set; }
	}

	[Serializable]
	public class BetTypeWinLossChoiceItem
	{
		public BettypeWinLossBySite Total { get; set; }

		public List<BettypeWinLossBySite> Choices { get; set; }
	}
}
