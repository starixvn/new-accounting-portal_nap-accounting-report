using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
	public class GetMatchesWithChoiceCriteria: CriteriaBase
	{
		[SpParam(SqlParamName = "Date")]
		public DateTime StartDate { get; set; }

		public string LeagueID { get; set; }

		public string SportBettype { get; set; }

		public string SportName { get; set; }

		public bool IsNoDraw { get; set; }

		public string SiteName { get; set; }

		public int LiveIndicator { get; set; }

		public int PermissionProfileID { get; set; }

		public string GlobalShowTime { get; set; }

		[SpParam(Ignored = true)]
		public string LeagueName { get; set; }

		public override string GetSettingKey() => "Acc_Rpt_LeagueWL_Match";

		public override bool IsValid() => true;
	}
}
