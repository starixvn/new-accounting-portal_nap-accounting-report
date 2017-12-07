using RemotingRef;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType221To225 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            return RefData.GetFastMarketsChoise(
                model.BetType.ToString(),
                model.BetTeam,
                (int)model.Hdp1,
                (int)model.Hdp2,
                "en");
        }
    }
}
