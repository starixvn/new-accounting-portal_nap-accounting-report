using RemotingRef;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType601And602And608And614 : ContentBetTeam
    {
        protected const string BasketballRefId = "WMBasketball";

        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            if (string.IsNullOrEmpty(model.BetTeam))
            {
                return string.Empty;
            }

            if (model.BetTeam == "D")
            {
                return RefData.GetResourceDataByRefIdAndResourceId("WMBasketball", "D", "en", string.Empty);
            }

            var isHome = model.BetTeam.Contains("H");
            var isAway = !isHome;
            var teamName = isHome ? model.HomeTeam : model.AwayTeam;

            var score = model.BetTeam.Replace("H", "").Replace("A", "");

            var format = RefData.GetResourceDataByRefIdAndResourceId(BasketballRefId, score, "en", string.Empty);
            var betType = RefData.GetBetTypeName("1", "en");
            return string.Format(format, teamName);
        }
    }
}
