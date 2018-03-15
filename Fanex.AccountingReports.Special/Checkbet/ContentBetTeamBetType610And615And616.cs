namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType610And615And616 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return model.BetTeam == "O" ? "Over" : "Under";
        }
    }
}
