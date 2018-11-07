namespace Fanex.BetList.Accounting.Services
{
    using System.Collections;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models.ScoreInformation;
    using Fanex.BetList.Accounting.Repositories;
    using Fanex.Common;
    using Fanex.Result.Core;
    using Fanex.Result.Core.Base;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreInformationService
    {
        private readonly ScoreInformationRepository _repository;

        public ScoreInformationService()
        {
            this._repository = new ScoreInformationRepository();
        }

        /// <summary>
        /// Gets the score information.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The score information.
        /// </returns>
        public ScoreInformation GetScoreInfo(GetScoreInformationCriteria criteria)
        {
            var scoreInformation = new ScoreInformation();
            Hashtable outputParams = new Hashtable();
            var sportsBooks = this._repository.GetScoreInformation(criteria, ref outputParams);
            scoreInformation.TraderName = Converter.ToString2(outputParams["tradername"]);
            scoreInformation.MatchId = criteria.MatchId;
            if (criteria.SportType != (int)SportType.Tennis || sportsBooks[0].GameDetails[0].MatchId == 0)
            {
                scoreInformation.MatchInformation = ResultEngine.HtmlParser(sportsBooks, this.GetFanexResultSetting(criteria)).Replace("original-title", " title ");
                return scoreInformation;
            }

            scoreInformation.MatchInformation = ResultEngine.HtmlGameDetailParser(sportsBooks[0], this.GetFanexResultSetting(criteria));

            return scoreInformation;
        }

        private ResultSetting GetFanexResultSetting(GetScoreInformationCriteria criteria)
        {
            return new ResultSetting
            {
                SportType = (SportType)criteria.SportType,
                IsOutright = false,
                IsParlayPopUp = false,
                IsPopUp = true,
                LanguageCode = "en-US"
            };
        }
    }
}