namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using Common;
    using Entities.LeagueWinLoss;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueWinLossRepository
    {
        /// <summary>
        /// Gets the league win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The league win loss.</returns>
        public List<LeagueWinLossEntity> GetLeagueWinLoss(ICriteria criteria)
        {
            DataSet result = null;
            var leagueWinLoss = new List<LeagueWinLossEntity>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new LeagueWinLossEntity
                    {
                        SportType = Converter.ToInt(row["SportType"]),
                        Sport = Converter.ToString2(row["Sport"]),
                        LeagueId = Converter.ToInt(row["LeagueID"]),
                        LeagueName = Converter.ToString2(row["LeagueName"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                        SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                        ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        RealIncome = Converter.ToDecimal(row["RealInCome"]),
                        VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                        RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"])
                    });
                }
            }

            return leagueWinLoss;
        }

        /// <summary>
        /// Gets the league win loss by sport.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="eightRMTable">The eight RM table.</param>
        /// <param name="barmTable">The BARM table.</param>
        /// <returns>The league win loss by sport.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<BettypeWinLossEntity> GetLeagueWinLossBySport(ICriteria criteria)
        {
            DataSet result = null;
            var bettypeWinLoss = new List<BettypeWinLossEntity>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null)
            {
                if (result.Tables.Count > 0)
                {
                    var winLossTable = result.Tables[0];

                    foreach (DataRow row in winLossTable.Rows)
                    {
                        bettypeWinLoss.Add(new BettypeWinLossEntity
                        {
                            LiveIndicator = Converter.ToInt(row["LiveIndicator"]) == 1,
                            Bettype = Converter.ToInt(row["Bettype"]),
                            BettypeName = Converter.ToString2(row["BettypeName"]),
                            IsHedge = Converter.ToInt(row["IsHedge"]) == 1,
                            MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                            SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                            ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                            Commission = Converter.ToDecimal(row["Commission"]),
                            RealIncome = Converter.ToDecimal(row["RealInCome"]),
                            VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                            RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                            VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"])
                        });
                    }
                }
            }

            return bettypeWinLoss;
        }

        /// <summary>
        /// Gets the league win loss by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The league win loss by bet type.</returns>
        public List<MatchEntity> GetLeagueWinLossByBetType(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<MatchEntity>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new MatchEntity
                    {
                        MatchCode = Converter.ToString2(row["MatchCode"]),
                        MatchEvent = Converter.ToString2(row["MatchEvent"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                        SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                        ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        RealIncome = Converter.ToDecimal(row["RealInCome"]),
                        VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                        RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"])
                    });
                }

                return leagueWinLoss;
            }

            return null;
        }

        public List<SportEntity> GetSport(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null && result.Tables.Count > 0)
            {
                var sportTable = result.Tables[0];
                var sport = new List<SportEntity>();
                foreach (DataRow row in sportTable.Rows)
                {
                    sport.Add(new SportEntity
                    {
                        SportGroupType = Converter.ToInt(row["SportGroupType"]),
                        SportGroupName = Converter.ToString2(row["SportGroupName"]),
                        SportType = Converter.ToInt(row["SportType"]),
                        SportName = Converter.ToString2(row["SportName"]),
                        Bettype = Converter.ToInt(row["Bettype"]),
                        BettypeName = Converter.ToString2(row["BettypeName"])
                    });
                }

                return sport;
            }

            return null;
        }
    }
}