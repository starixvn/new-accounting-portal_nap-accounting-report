namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetTypeYesNo : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "Y" ? "Yes" : "No";
        }
    }
}
