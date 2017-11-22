using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType422To423 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("H", "Home");
            team.Add("A", "Away");
            team.Add("N", "Neither");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
