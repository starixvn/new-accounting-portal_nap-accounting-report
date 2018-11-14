namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetTypeAsBetTeam : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam;
        }
    }
}