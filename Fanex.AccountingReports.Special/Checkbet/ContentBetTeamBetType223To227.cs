using RemotingRef;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType223To227 : ContentBetTeam
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
