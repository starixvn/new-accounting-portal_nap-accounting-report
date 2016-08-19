using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType424 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("S", "Shot");
            team.Add("H", "Header");
            team.Add("P", "Penalty");
            team.Add("FK", "Free Kick");
            team.Add("OG", "Own Goal");
            team.Add("NG", "No Goal");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
