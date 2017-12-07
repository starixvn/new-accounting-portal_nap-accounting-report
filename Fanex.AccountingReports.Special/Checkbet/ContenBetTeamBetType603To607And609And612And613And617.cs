namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType603To607And609And612And613And617 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "H" ? model.HomeTeam : model.AwayTeam;
        }
    }
}
