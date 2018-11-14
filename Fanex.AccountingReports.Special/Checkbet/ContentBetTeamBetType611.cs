namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType611 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Odd" : "Even";
        }
    }
}