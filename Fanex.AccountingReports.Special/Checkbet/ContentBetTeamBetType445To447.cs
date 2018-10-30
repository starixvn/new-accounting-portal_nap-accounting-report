using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType445To447 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("YY", "Yes/Yes");
            team.Add("YN", "Yes/No");
            team.Add("NY", "No/Yes");
            team.Add("NN", "No/No");

            return team.ContainsKey(model.BetTeam) ? team[model.BetTeam] : string.Empty;
        }
    }
}