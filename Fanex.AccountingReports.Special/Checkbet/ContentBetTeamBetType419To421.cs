using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType419To421 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("1H", "First Half");
            team.Add("2H", "Second Half");
            team.Add("N", "Neither");

            return team.ContainsKey(model.BetTeam) ? team[model.BetTeam] : string.Empty;
        }
    }
}
