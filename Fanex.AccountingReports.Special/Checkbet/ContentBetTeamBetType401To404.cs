namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType401To404 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Over" : "Under";
        }
    }
}