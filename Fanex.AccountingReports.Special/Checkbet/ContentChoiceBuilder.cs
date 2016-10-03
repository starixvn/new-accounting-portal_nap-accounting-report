using System;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentChoiceBuilder
    {
        public string GetContentBetchoiceToShowOf(
            string homeTeam,
            string awayTeam,
            int betType,
            string betTeam,
            decimal hdp1)
        {
            string result = string.Empty;
            var model = new BetTeamModel
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                BetType = betType,
                BetTeam = betTeam.ToUpper(),
                Hdp1 = decimal.Round(hdp1, 2, MidpointRounding.AwayFromZero)
            };

            var bettypeMapper = new ContentBetTypeMapper();
            string bettypeContentClass = bettypeMapper.GetContentBetchoiceToShowOf(model.BetType);
            if (!string.IsNullOrWhiteSpace(bettypeContentClass))
            {
                string typeName = string.Format("Fanex.AccountingReports.Special.Checkbet.{0}", bettypeContentClass);
                Type type = Type.GetType(typeName);
                if (type == null)
                {
                    type = Type.GetType("Fanex.AccountingReports.Special.Checkbet.ContenBetTeamBetTypeNA");
                }

                ContentBetTeam content = Activator.CreateInstance(type) as ContentBetTeam;
                result = content.ContentBetTeamBuilder(model);
            }
            else
            {
                result = betTeam
                    .Replace("-over", "+")
                    .Replace("-Over", "+")
                    .Replace("&over", "+")
                    .Replace("G6", "6+")
                    .Replace("G3", "3+");
            }

            return result;
        }
    }
}
