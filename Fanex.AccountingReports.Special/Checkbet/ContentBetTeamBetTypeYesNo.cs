namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetTypeYesNo : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "Y" ? "Yes" : "No";
        }
    }
}
