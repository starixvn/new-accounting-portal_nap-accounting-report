namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetTypeAsBetTeam : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam;
        }
    }
}
