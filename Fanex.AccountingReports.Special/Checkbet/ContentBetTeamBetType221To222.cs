using RemotingRef;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType221To222 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var choice = RefData.GetFastMarketsChoise(
                model.BetType.ToString(),
                model.BetTeam,
                (int)model.Hdp1,
                (int)model.Hdp2,
                "en");

            return $"{choice} {model.OddsSpread.ToString("G29")}";
        }
    }
}
