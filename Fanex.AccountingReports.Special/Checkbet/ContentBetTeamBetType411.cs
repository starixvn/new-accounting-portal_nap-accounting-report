namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType411 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "H" ? model.HomeTeam : model.AwayTeam;
        }
    }
}