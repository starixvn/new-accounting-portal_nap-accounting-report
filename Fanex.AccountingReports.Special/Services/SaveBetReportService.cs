namespace Fanex.AccountingReports.Special.Services.Implement.SaveBetReport
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Models.SaveBetReport;
    using Fanex.Common;
    using Fanex.Data;
    using RemotingRef;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SaveBetReportService : ServiceBase
    {
        public SaveBetReportService()
        {
        }

        /// <summary>
        /// Gets the sports.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Sports data.</returns>
        public Sport GetSports(GetSportsCriteria criteria)
        {
            var result = new Sport();
            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var _params = criteria;
                var reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    var sportInfo = new Sport
                    {
                        SportType = Converter.ToInt(reader["SportType"]),
                        SportName = Converter.ToString2(reader["SportName"])
                    };

                    result.Sports.Add(sportInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the leagues.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Leagues data.</returns>
        public League GetLeagues(GetLeaguesCriteria criteria)
        {
            var result = new League();

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var _params = criteria;
                var reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    var leagueInfo = new League
                    {
                        LeagueId = Converter.ToInt(reader["leagueid"]),
                        LeagueName = Converter.ToString2(reader["leaguename"])
                    };

                    result.Leagues.Add(leagueInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the match data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List match data.</returns>
        public Match GetMatchs(GetMatchsCriteria criteria)
        {
            var result = new Match();

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var _params = criteria;
                var reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    var matchInfo = new Match()
                    {
                        MatchId = Converter.ToInt(reader["MatchID"]),
                        MatchCode = new RemotingRefService().GetBetRadarMatchCodeFormat(Converter.ToString2(reader["MatchCode"]), "en"),
                        HomeTeam = Converter.ToString2(reader["Home"]),
                        AwayTeam = Converter.ToString2(reader["Away"]),
                        EventDate = Converter.ToDateTime(reader["EventDate"]).ToShortFormatedDateWithSlash().Replace("/", "_"),

                        SportType = Converter.ToInt(reader["SportType"])
                    };

                    result.Matchs.Add(matchInfo);
                }
            }

            return result;
        }
    }
}