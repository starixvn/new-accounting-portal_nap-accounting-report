// --------------------------------------------------------------------------
// <copyright file="ScoreInformationService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections;
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models.ScoreInformation;
    using Fanex.BetList.Accounting.Repositories;
    using Fanex.Common;
    using Fanex.Result.Core;
    using Fanex.Result.Core.Base;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreInformationService : IScoreInformationService
    {
        private readonly IScoreInformationRepository _repository;

        public ScoreInformationService()
            : this(null)
        {
        }

        public ScoreInformationService(IScoreInformationRepository repository)
        {
            this._repository = repository ?? new ScoreInformationRepository();
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
            ScoreInformation scoreInformation = new ScoreInformation();
            try
            {
                Hashtable outputParams = new Hashtable();
                var sportsBooks = this._repository.GetScoreInformation(criteria, ref outputParams);
                scoreInformation.TraderName = Converter.ToString2(outputParams["tradername"]);
                if (criteria.SportType != (int)SportType.Tennis || sportsBooks[0].GameDetails[0].MatchId == 0)
                {
                    scoreInformation.MatchInformation = ResultEngine.HtmlParser(sportsBooks, this.GetFanexResultSetting(criteria)).Replace("original-title", " title ");
                    return scoreInformation;
                }

                scoreInformation.MatchInformation = ResultEngine.HtmlGameDetailParser(sportsBooks[0], this.GetFanexResultSetting(criteria));
            }
            catch (Exception exception)
            {
                LoggingHandler.Log(exception);
            }

            return scoreInformation;
        }

        private ResultSetting GetFanexResultSetting(GetScoreInformationCriteria criteria)
        {
            return new ResultSetting
            {
                SportType = (SportType)criteria.SportType,
                IsOutRight = false,
                IsParlayPopUp = false,
                IsPopUp = true,
                LanguageCode = "en-US"
            };
        }
    }
}