namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using Common;
    using Entities.LeagueWinLoss;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System.Collections;
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
        public List<dynamic> GetLeagueWinLossBySport(
            ICriteria criteria,
            out List<dynamic> eightRMTable,
            out List<dynamic> barmTable)
        {
            DataSet result = null;
            var leagueWinLoss = new List<dynamic>();
            eightRMTable = new List<dynamic>();
            barmTable = new List<dynamic>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null)
            {
                if (result.Tables.Count >= 1)
                {
                    var winLossTable = result.Tables[0];

                    foreach (DataRow row in winLossTable.Rows)
                    {
                        leagueWinLoss.Add(new
                        {
                            liveindicator = row["liveindicator"],
                            Bettype = row["Bettype"],
                            isHedge = row["isHedge"],
                            WinLost = row["WinLost"],
                            Position = row["Position"],
                            Commission = row["Commission"],
                            LineTotal = row["LineTotal"],
                            Turnover = row["Turnover"]
                        });
                    }
                }

                if (result.Tables.Count >= 2)
                {
                    var raw8RMTable = result.Tables[1];
                    foreach (DataRow row in raw8RMTable.Rows)
                    {
                        eightRMTable.Add(new
                        {
                            liveindicator = row["liveindicator"],
                            Winlost8RM = row["Winlost8RM"]
                        });
                    }
                }

                if (result.Tables.Count >= 3)
                {
                    var rawBARMTable = result.Tables[2];
                    foreach (DataRow row in rawBARMTable.Rows)
                    {
                        barmTable.Add(new
                        {
                            liveindicator = row["liveindicator"],
                            WinlostBARM = row["WinlostBARM"]
                        });
                    }
                }
            }

            return leagueWinLoss;
        }

        /// <summary>
        /// Gets the league win loss by bet type.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The league win loss by bet type.</returns>
        public List<dynamic> GetLeagueWinLossByBetType(ICriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<dynamic>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new
                    {
                        matchcode = row["matchcode"],
                        HomeId = row["HomeId"],
                        AwayId = row["AwayId"],
                        liveindicator = row["liveindicator"],
                        BetType = row["BetType"],
                        leagueid = row["leagueid"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        IsNeutral = row["IsNeutral"]
                    });
                }

                return leagueWinLoss;
            }

            return null;
        }

        public List<dynamic> GetSport(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
            }

            if (result != null && result.Tables.Count > 0)
            {
                var sportTable = result.Tables[0];
                var sport = new List<dynamic>();
                foreach (DataRow row in sportTable.Rows)
                {
                    sport.Add(new
                    {
                        SportGroupType = row["SportGroupType"],
                        SportGroupName = row["SportGroupName"],
                        SportType = row["SportType"],
                        SportName = row["SportName"]
                    });
                }

                return sport;
            }

            return null;
        }
    }
}