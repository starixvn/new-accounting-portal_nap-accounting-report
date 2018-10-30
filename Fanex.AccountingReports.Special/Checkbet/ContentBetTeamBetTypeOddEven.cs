namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetTypeOddEven : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Odd" : "Even";
        }
    }
}