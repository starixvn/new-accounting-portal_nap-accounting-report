namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType411 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "H" ? model.HomeTeam : model.AwayTeam;
        }
    }
}
