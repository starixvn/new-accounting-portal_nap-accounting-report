namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType601And602And608And614 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            if (string.IsNullOrEmpty(model.BetTeam))
            {
                return string.Empty;
            }

            if (model.BetTeam == "D")
            {
                return "Draw";
            }

            var isHome = model.BetTeam.Contains("H");
            var isAway = !isHome;
            var teamName = isHome ? model.HomeTeam : model.AwayTeam;

            var score = model.BetTeam.Replace("+", "")
                   .Replace("H", "")
                   .Replace("A", "");


            if (model.BetTeam.Contains("+"))
            {
                return $"{teamName} to Win by {score}+ points";
            }
            else
            {
                var byText = score.Replace("-", " to ");
                return $"{teamName} to Win by {byText} points";
            }
        }
    }
}
