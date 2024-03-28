using Fanex.Data.Repository;
using System;
using Fanex.Data;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
	public class GetBetTypesWithChoiceCriteria : CriteriaBase
	{
		[SpParam(SqlParamName = "Date")]
		public DateTime StartDate { get; set; }

		public int LiveIndicator { get; set; }

		public int MatchID { get; set; }

		public string SportBettype { get; set; }

		public string SportName { get; set; }

		public bool IsNoDraw { get; set; }

		public string SiteName { get; set; }

		public int PermissionProfileID { get; set; }

		public override string GetSettingKey() => "Acc_Rpt_LeagueWL_BetType";

		public override bool IsValid() => true;
	}
}
