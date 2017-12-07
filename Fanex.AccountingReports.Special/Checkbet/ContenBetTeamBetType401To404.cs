namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType401To404 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Over" : "Under";
        }
    }
}
