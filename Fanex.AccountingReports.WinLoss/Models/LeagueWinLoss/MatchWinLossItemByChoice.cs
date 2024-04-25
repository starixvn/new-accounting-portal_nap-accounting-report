using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss
{
	[Serializable]
	public class MatchWinLossItemByChoice
	{
		public MatchWinLossByLiveStatus Total { get; set; }

		public List<MatchWinLossLiveIndicatorItem> LiveIndicators { get; set; }
	}

	[Serializable]
	public class MatchWinLossLiveIndicatorItem
	{
		public MatchWinLossByLiveStatus Total { get; set; }

		public List<MatchWinLossLiveBetTypeItem> BetTypes { get; set; }
	}

	[Serializable]
	public class MatchWinLossLiveBetTypeItem
	{
		public MatchWinLossByLiveStatus Total { get; set; }

		public List<MatchWinLossByLiveStatus> Choices { get; set; }
	}
}
