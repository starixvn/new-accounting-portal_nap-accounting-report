using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType418 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("YO", "Yes & Over" + " " + model.Hdp1.ToString());
            team.Add("YU", "Yes & Under" + " " + model.Hdp1.ToString());
            team.Add("NO", "No & Over" + " " + model.Hdp1.ToString());
            team.Add("NU", "No & Under" + " " + model.Hdp1.ToString());

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
