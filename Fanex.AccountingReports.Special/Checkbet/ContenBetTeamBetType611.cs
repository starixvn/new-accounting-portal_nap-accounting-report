namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType611 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Odd" : "Even";
        }
    }
}
