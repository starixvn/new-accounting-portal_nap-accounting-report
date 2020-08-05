namespace Fanex.BetList.Accounting.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Result.Core.Sportsbook.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ScoreInformationRepository
    {
        /// <summary>
        /// Gets the score information.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParams">The output parameters.</param>>
        /// <returns>
        /// The score information.
        /// </returns>
        public List<Sportsbook> GetScoreInformation(
            GetScoreInformationCriteria criteria,
            ref Hashtable outputParams)
        {
            var result = new List<Sportsbook>();

            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.SetParameterValue("MatchID", criteria.MatchId);
                objDb.SetParameterValue("Bettype", criteria.Bettype);
                objDb.SetParameterValue("tradername", "Unassigned");
                objDb.SetParameterValue("LiveIndicator", criteria.LiveIndicator);

                using (var reader = objDb.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Add(BindSportsbook(reader, criteria));
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                result[0].GameDetails.Add(BindGameDetail(reader));
                            }
                        }

                        if (result[0].GameDetails.Count == 0)
                        {
                            result[0].GameDetails.Add(new GameDetail());
                        }
                    }
                }

                outputParams = objDb.GetOuputParams();
            }

            return result;
        }

        /// <summary>
        /// Binds the SPORTSBOOK.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The object of SPORTSBOOK.</returns>
        private static Sportsbook BindSportsbook(IDataReader reader, GetScoreInformationCriteria criteria)
        {
            return new Sportsbook()
            {
                HomeId = Converter.ToInt(reader["homeID"]),
                AwayId = Converter.ToInt(reader["awayID"]),
                HtHomeScore = Converter.ToInt(reader["hthomescore"]),
                FinalHomeScore = Converter.ToInt(reader["finalhomescore"]),
                HtAwayScore = Converter.ToInt(reader["htawayscore"]),
                FinalAwayScore = Converter.ToInt(reader["finalawayscore"]),
                EventStatus = Converter.ToString2(reader["eventstatus"]),
                KickOffTime = Converter.ToDateTime(reader["kickofftime"].ToString()),
                EventDate = Converter.ToDateTime(reader["eventdate"].ToString()),
                IsNeutral = Converter.ToBool(reader["IsNeutral"]),
                BetStatus = Converter.ToString2(reader["betstatus"]),
                SettlementStatus = (byte)Converter.ToInt(reader["SettlementStatus"]),
                LeagueId = criteria.LeagueId,
                MatchId = criteria.MatchId,
                Color = string.Empty,
                MatchCode = criteria.MatchCode,
                GoalRecord = Converter.ToString2(reader["goalrecord"]),
                IsHTCal = Converter.ToBool(reader["IsHTCal"]),
                Calculate = Converter.ToBool(reader["Calculate"]),
                Bettype = criteria.Bettype,
                RefNo = string.Empty,
                GameDetails = new List<GameDetail>(),
                LiveHomeScore = Converter.ToInt(reader["livehomescore"]),
                LiveAwayScore = Converter.ToInt(reader["liveawayscore"])
            };
        }

        /// Bind game detail value.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <returns>The game detail value.</returns>
        private static GameDetail BindGameDetail(IDataReader reader)
        {
            return new GameDetail
            {
                MatchId = int.Parse(reader["matchid"].ToString()),
                ResourceId = reader["ResourceId"].ToString(),
                HomeValue = reader["HomeValue"].ToString(),
                AwayValue = reader["AwayValue"].ToString(),
                ResultStatusID = int.Parse(reader["ResultStatusID"].ToString()),
                SettlementStatusID = int.Parse(reader["SettlementStatusID"].ToString()),
                OtherInfo = Converter.ToString2(reader["OtherInfo"])
            };
        }
    }
}