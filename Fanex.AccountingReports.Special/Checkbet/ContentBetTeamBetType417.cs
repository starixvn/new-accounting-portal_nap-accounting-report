using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType417 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("YH", "Yes/Home");
            team.Add("YA", "Yes/Away");
            team.Add("NH", "No/Home");
            team.Add("NA", "No/Away");
            team.Add("YD", "Yes/Draw");
            team.Add("ND", "No/Draw");

            return team.ContainsKey(model.BetTeam) ? team[model.BetTeam] : string.Empty;
        }
    }
}