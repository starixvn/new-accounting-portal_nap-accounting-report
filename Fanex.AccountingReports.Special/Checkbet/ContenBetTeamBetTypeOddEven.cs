namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetTypeOddEven : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Odd" : "Even";
        }
    }
}
