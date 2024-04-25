namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using Common;
    using Entities.LeagueWinLoss;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;

    public class LeagueWinLossRepository
    {
        public List<LeagueWinLossEntity> GetLeagueWinLoss(ICriteria criteria)
        {
            DataSet result = null;
            var leagueWinLoss = new List<LeagueWinLossEntity>();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                foreach (DataRow row in winLossTable.Rows)
                {
                    var entity = new LeagueWinLossEntity
                    {
                        SportType = Converter.ToInt(row["SportType"]),
                        Sport = Converter.ToString2(row["Sport"]),
                        LeagueName = Converter.ToString2(row["LeagueName"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                        SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                        ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        RealIncome = Converter.ToDecimal(row["RealInCome"]),
                        VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                        RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"]),
                        IsSabaSport = Converter.ToInt(row["IsSabaSport"]),
                        LeagueId = Converter.ToString2(row["LeagueId"])
                    };

                    if(row["MatchCount"] == DBNull.Value)
                    {
                        entity.MatchCount = null;
                    }
                    else
                    {
                        entity.MatchCount = Converter.ToInt(row["MatchCount"]);
                    }

                    leagueWinLoss.Add(entity);
                }
            }

            return leagueWinLoss;
        }

        
        public List<BettypeWinLossEntity> GetBetTypes(ICriteria criteria, bool isNormal)
        {
            DataSet result = null;
            var bettypeWinLoss = new List<BettypeWinLossEntity>();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
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
                            Bettype = Converter.ToInt(row["BetTypeID"]),
                            BettypeName = Converter.ToString2(row["BetTypeName"]),
                            SiteType = Converter.ToString2(row["SiteType"]),
                            MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                            SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                            ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                            Commission = Converter.ToDecimal(row["Commission"]),
                            RealIncome = Converter.ToDecimal(row["RealInCome"]),
                            VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                            RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                            VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"]),
                            IsHedge = isNormal || Converter.ToInt(row["IsHedge"]) == 1,
                            TraderName = Converter.ToString2(row["TraderName"]),
                            Choice = isNormal ? Converter.ToString2(row["Choice"]) : string.Empty
                        });
                    }
                }
            }

            return bettypeWinLoss;
        }

        public List<MatchEntity> GetMatches(ICriteria criteria, bool isNormalMode)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<MatchEntity>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new MatchEntity
                    {
                        MatchID = Converter.ToInt(row["MatchID"]),
                        MatchCode = Converter.ToString2(row["MatchCode"]),
                        LiveIndicator = Converter.ToInt(row["LiveIndicator"]) == 1,
                        MatchEvent = Converter.ToString2(row["MatchEvent"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                        SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                        ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        RealIncome = Converter.ToDecimal(row["RealInCome"]),
                        VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                        RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"]),
                        IsHedge = Converter.ToString2(row["MatchCode"]).Contains("hedge"),
                        IsOutright = Converter.ToInt(row["IsOutright"]) == 1,
                        BetTypeName = isNormalMode ? Converter.ToString2(row["BetTypeName"]) : string.Empty,
                        Choice = isNormalMode ? Converter.ToString2(row["Choice"]) : string.Empty,
                        BetType = isNormalMode ? Converter.ToInt(row["BetType"]) : 0
                    });
                }

                return leagueWinLoss;
            }

            return null;
        }

        public List<SiteEntity> GetSites(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<SiteEntity>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new SiteEntity
                    {
                        SiteID = Converter.ToInt(row["SiteID"]),
                        SiteName = Converter.ToString2(row["SiteName"]),
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

        public List<SuperEntity> GetSupers(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<SuperEntity>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new SuperEntity
                    {
                        SuperID = Converter.ToInt(row["SuperID"]),
                        SuperName = Converter.ToString2(row["SuperName"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWinLoss"]),
                        SMAWinLoss = Converter.ToDecimal(row["SMAWinLoss"]),
                        ExtraPT = Converter.ToDecimal(row["ExtraPT"]),
                        Commission = Converter.ToDecimal(row["Commission"]),
                        RealIncome = Converter.ToDecimal(row["RealInCome"]),
                        VirtualWinLoss = Converter.ToDecimal(row["VirtualIncome"]),
                        RealTurnover = Converter.ToDecimal(row["RealTurnover"]),
                        VirtualTurnover = Converter.ToDecimal(row["VirtualTurnover"]),
                        HasVR = Converter.ToBool(row["HasVR"])
                    });
                }

                return leagueWinLoss;
            }

            return null;
        }

        public List<VirtualRateEntity> GetVirtualRate(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                var leagueWinLoss = new List<VirtualRateEntity>();
                foreach (DataRow row in winLossTable.Rows)
                {
                    leagueWinLoss.Add(new VirtualRateEntity
                    {
                        Sport = Converter.ToString2(row["sport"]),
                        BetType = Converter.ToString2(row["bettype"]),
                        LiveStatus = Converter.ToString2(row["islive"]),
                        VirtualRate = Converter.ToDecimal(row["virtualMultiplier"])
                    });
                }

                return leagueWinLoss;
            }

            return null;
        }

        public List<SportEntity> GetSport(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
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
                        BetTypeGroupName = Converter.ToString2(row["BetGroupName"]),
                        Bettype = Converter.ToInt(row["Bettype"]),
                        BettypeName = Converter.ToString2(row["BettypeName"])
                    });
                }

                return sport;
            }

            return null;
        }

        public List<SiteFilterEntity> GetSiteFilter(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0)
            {
                var siteTable = result.Tables[0];
                var sites = new List<SiteFilterEntity>();
                foreach (DataRow row in siteTable.Rows)
                {
                    sites.Add(new SiteFilterEntity
                    {
                        SiteGroup = Converter.ToString2(row["SiteGroup"]),
                        SiteName = Converter.ToString2(row["SiteName"]),
                        SiteLabel = Converter.ToString2(row["SiteLabel"])
                    });
                }

                return sites;
            }

            return null;
        }

        public int GetPermissionProfileID(ICriteria criteria)
        {
            DataSet result = null;

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
            }

            if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                var firstRow = result.Tables[0].Rows[0];
                var permissionProfileId = Converter.ToInt(firstRow["PermisionProfileID"]);

                return permissionProfileId;
            }

            return 0;
        }
    }
}
