namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Trader performance soccer repository.
    /// </summary>
    public class TraderPerformanceSoccerRepository
    {
        /// <summary>
        /// Gets the trader performance soccer report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Trader performance for soccer data.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public TraderPerformanceSoccer GetTraderPerformanceSoccerReport(GetReportCriteria criteria)
        {
            var result = new TraderPerformanceSoccer
            {
                GrandTotal = new TraderPerformanceSoccer()
            };
            var settingKey = criteria.GetSettingKey();

            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                var winLossGrandTotal = new TraderPerformanceSoccer();
                var turnOverGrandTotal = new TraderPerformanceSoccer();
                var liveBetCountGrandTotal = new TraderPerformanceSoccer();
                var marginGrandTotal = new TraderPerformanceSoccer();
                var commissionGrandTotal = new TraderPerformanceSoccer();

                var param = criteria;
                var reader = objectDb.ExecuteReader(param);

                if (reader == null)
                {
                    return result;
                }

                while (reader.Read())
                {
                    if (Converter.ToInt(reader["custid"]) == 0 &&
                        Converter.ToDecimal(reader["overall_st"]) == 0)
                    {
                        continue;
                    }

                    TraderPerformanceSoccer traderInfo = new TraderPerformanceSoccer
                    {
                        Id = Converter.ToInt(reader["custid"]),
                        Name = Converter.ToString2(reader["username"]),
                        MatchNumber = Converter.ToInt(reader["matchno"]),
                        Major = Converter.ToInt(reader["Major"]),
                        Minor1 = Converter.ToInt(reader["Minor1"]),
                        Minor2 = Converter.ToInt(reader["Minor2"]),
                        Minor3 = Converter.ToInt(reader["Minor3"]),
                        Others = Converter.ToInt(reader["Others"])
                    };

                    var winLossInfo = GetTraderGroupInfo(reader, "winlost", "wl");
                    var turnOverInfo = GetTraderGroupInfo(reader, "stake", "st");
                    var commissionInfo = GetTraderGroupInfo(reader, "comm", "com");
                    var marginInfo = GetTraderGroupInfo(reader, "margin", "margin");
                    marginInfo.GetMarginValue(marginInfo);
                    var liveBetCountInfo = GetTraderGroupInfo(reader, "bet", "livebet");

                    winLossGrandTotal.CalculateTotal(winLossInfo);
                    turnOverGrandTotal.CalculateTotal(turnOverInfo);
                    commissionGrandTotal.CalculateTotal(commissionInfo);
                    liveBetCountGrandTotal.CalculateTotal(liveBetCountInfo);

                    result.GrandTotal.CalculateTotalMatchAndLeague(
                        traderInfo.MatchNumber,
                        traderInfo.Major,
                        traderInfo.Minor1,
                        traderInfo.Minor2,
                        traderInfo.Minor3,
                        traderInfo.Others);

                    winLossInfo.FormatTwoDecimalNumberData(winLossInfo);
                    turnOverInfo.FormatTwoDecimalNumberData(turnOverInfo);
                    marginInfo.FormatTwoDecimalNumberData(marginInfo);
                    commissionInfo.FormatTwoDecimalNumberData(commissionInfo);

                    traderInfo.ChildInfo.Add(winLossInfo);
                    traderInfo.ChildInfo.Add(turnOverInfo);
                    traderInfo.ChildInfo.Add(liveBetCountInfo);
                    traderInfo.ChildInfo.Add(marginInfo);
                    traderInfo.ChildInfo.Add(commissionInfo);

                    result.ReportRows.Add(traderInfo);
                }

                marginGrandTotal.CalculateMarginTotal(winLossGrandTotal, turnOverGrandTotal);

                winLossGrandTotal.FormatTwoDecimalNumberData(winLossGrandTotal);
                turnOverGrandTotal.FormatTwoDecimalNumberData(turnOverGrandTotal);
                marginGrandTotal.FormatTwoDecimalNumberData(marginGrandTotal);
                commissionGrandTotal.FormatTwoDecimalNumberData(commissionGrandTotal);

                result.GrandTotal.ChildInfo.Add(winLossGrandTotal);
                result.GrandTotal.ChildInfo.Add(turnOverGrandTotal);
                result.GrandTotal.ChildInfo.Add(liveBetCountGrandTotal);
                result.GrandTotal.ChildInfo.Add(marginGrandTotal);
                result.GrandTotal.ChildInfo.Add(commissionGrandTotal);
            }

            return result;
        }

        /// <summary>
        /// Gets the trader performance soccer detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Trader performance for soccer detail data.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public TraderPerformanceSoccerDetail GetTraderPerformanceSoccerDetailReport(GetDetailReportCriteria criteria)
        {
            var result = new TraderPerformanceSoccerDetail
            {
                GrandTotal = new TraderPerformanceSoccerDetail()
            };

            var winLossTotal = new TraderPerformanceSoccerDetail();
            var turnOverTotal = new TraderPerformanceSoccerDetail();
            var liveBetCountTotal = new TraderPerformanceSoccerDetail();
            var marginTotal = new TraderPerformanceSoccerDetail();
            var commissionTotal = new TraderPerformanceSoccerDetail();

            var settingKey = criteria.GetSettingKey();

            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                var param = criteria;
                var reader = objectDb.ExecuteReader(param);

                if (reader == null)
                {
                    return result;
                }

                while (reader.Read())
                {
                    var eventDate = Converter.ToDateTime(reader["eventdate"]);
                    TraderPerformanceSoccerDetail traderInfo = new TraderPerformanceSoccerDetail
                    {
                        Id = Converter.ToInt(reader["custid"]),
                        Name = Converter.ToString2(reader["username"]),
                        EventDate = DateTimeExtensions.ToMonthAndDayWithNoHyphen(eventDate),
                        LeagueId = Converter.ToInt(reader["leagueid"]),
                        HomeId = Converter.ToInt(reader["homeid"]),
                        AwayId = Converter.ToInt(reader["awayid"]),
                        IsNeutral = Converter.ToBool(reader["IsNeutral"]),
                        HTHomeScore = Converter.ToString2(reader["hthomescore"]),
                        HTAwayScore = Converter.ToString2(reader["htawayscore"]),
                        FTHomeScore = Converter.ToString2(reader["fthomescore"]),
                        FTAwayScore = Converter.ToString2(reader["ftawayscore"]),
                        Maxbet = Converter.ToDecimal(reader["maxbet"])
                    };

                    var winLossInfo = GetTraderGroupDetailInfo(reader, "winlost", "wl");
                    var turnOverInfo = GetTraderGroupDetailInfo(reader, "stake", "st");
                    var commissionInfo = GetTraderGroupDetailInfo(reader, "comm", "com");
                    var marginInfo = GetTraderGroupDetailInfo(reader, "margin", "margin");
                    marginInfo.GetMarginValue(marginInfo);
                    var liveBetCountInfo = GetTraderGroupDetailInfo(reader, "bet", "livebet");

                    traderInfo.LeagueName = CacheRef.GetLeagueName(traderInfo.LeagueId);
                    traderInfo.HomeName = CacheRef.GetTeamName(traderInfo.HomeId);
                    traderInfo.AwayName = CacheRef.GetTeamName(traderInfo.AwayId);

                    traderInfo.ChildInfo.Add(winLossInfo);
                    traderInfo.ChildInfo.Add(turnOverInfo);
                    traderInfo.ChildInfo.Add(liveBetCountInfo);
                    traderInfo.ChildInfo.Add(marginInfo);
                    traderInfo.ChildInfo.Add(commissionInfo);

                    winLossTotal.CalculateTotal(winLossInfo);
                    turnOverTotal.CalculateTotal(turnOverInfo);
                    liveBetCountTotal.CalculateTotal(liveBetCountInfo);
                    commissionTotal.CalculateTotal(commissionInfo);

                    result.ReportRows.Add(traderInfo);
                }

                marginTotal.CalculateMarginTotal(winLossTotal, turnOverTotal);

                winLossTotal.FormatTwoDecimalNumberData(winLossTotal);
                turnOverTotal.FormatTwoDecimalNumberData(turnOverTotal);
                marginTotal.FormatTwoDecimalNumberData(marginTotal);
                commissionTotal.FormatTwoDecimalNumberData(commissionTotal);

                result.GrandTotal.ChildInfo.Add(winLossTotal);
                result.GrandTotal.ChildInfo.Add(turnOverTotal);
                result.GrandTotal.ChildInfo.Add(liveBetCountTotal);
                result.GrandTotal.ChildInfo.Add(marginTotal);
                result.GrandTotal.ChildInfo.Add(commissionTotal);
            }

            return result;
        }

        /// <summary>
        /// Gets the trader group information.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="groupNamePrefix">The group name prefix.</param>
        /// <param name="overallSuffix">The overall suffix.</param>
        /// <returns>Trader group information.</returns>
        private TraderPerformanceSoccer GetTraderGroupInfo(
           IDataReader reader,
           string groupNamePrefix,
           string overallSuffix)
        {
            var traderGroupInfo = new TraderPerformanceSoccer
            {
                Hdp = Converter.ToDecimal(reader[groupNamePrefix + "_hdp"]),
                FirstHalfHdp = Converter.ToDecimal(reader[groupNamePrefix + "_1sthdp"]),
                FifteenMinHdp = Converter.ToDecimal(reader[groupNamePrefix + "_15minhdp"]),
                FirstHalfOU = Converter.ToDecimal(reader[groupNamePrefix + "_1stou"]),
                OU = Converter.ToDecimal(reader[groupNamePrefix + "_ou"]),
                FirstHalfOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_1st1x2"]),
                OneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_1x2"]),
                FifteenMinOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_15min1x2"]),
                FirstHalfNoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_1stNoOfCorners"]),
                NoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_NoOfCorners"]),
                FifteenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_15minOU"]),
                FirstHalfTotalBookings = Converter.ToDecimal(reader[groupNamePrefix + "_1stTotalBookings"]),
                TotalBookings = Converter.ToDecimal(reader[groupNamePrefix + "_TotalBookings"]),
                NextCorner = Converter.ToDecimal(reader[groupNamePrefix + "_NextCorner"]),
                Overall = Converter.ToDecimal(reader["overall_" + overallSuffix]),
                NextBookings = Converter.ToDecimal(reader[groupNamePrefix + "_NextBooking"]),
                TotalBookingsFifteenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_15minTotalBookings"]),
                TotalBookingsFifteenMinHDP = Converter.ToDecimal(reader[groupNamePrefix + "_15minTotalBookingsHdp"]),
                TenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_10minOU"]),
                FifteenMinNoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_15minCorners"]),
                SingleTeamOU = Converter.ToDecimal(reader[groupNamePrefix + "_SingleTeamOU"]),
                TenMinOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_10min1x2"]),
                TenMinHdp = Converter.ToDecimal(reader[groupNamePrefix + "_10minHdp"]),
            };

            return traderGroupInfo;
        }

        /// <summary>
        /// Gets the trader group detail information.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="groupNamePrefix">The group name prefix.</param>
        /// <param name="overallSuffix">The overall suffix.</param>
        /// <returns>Trader group detail information.</returns>
        private TraderPerformanceSoccerDetail GetTraderGroupDetailInfo(
            IDataReader reader,
            string groupNamePrefix,
            string overallSuffix)
        {
            var traderGroupInfo = new TraderPerformanceSoccerDetail
            {
                Hdp = Converter.ToDecimal(reader[groupNamePrefix + "_hdp"]),
                FirstHalfHdp = Converter.ToDecimal(reader[groupNamePrefix + "_1sthdp"]),
                FifteenMinHdp = Converter.ToDecimal(reader[groupNamePrefix + "_15minhdp"]),
                FirstHalfOU = Converter.ToDecimal(reader[groupNamePrefix + "_1stou"]),
                OU = Converter.ToDecimal(reader[groupNamePrefix + "_ou"]),
                FirstHalfOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_1st1x2"]),
                OneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_1x2"]),
                FifteenMinOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_15min1x2"]),
                FirstHalfNoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_1stNoOfCorners"]),
                NoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_NoOfCorners"]),
                FifteenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_15minOU"]),
                FirstHalfTotalBookings = Converter.ToDecimal(reader[groupNamePrefix + "_1stTotalBookings"]),
                TotalBookings = Converter.ToDecimal(reader[groupNamePrefix + "_TotalBookings"]),
                NextCorner = Converter.ToDecimal(reader[groupNamePrefix + "_NextCorner"]),
                Overall = Converter.ToDecimal(reader["overall_" + overallSuffix]),
                NextBookings = Converter.ToDecimal(reader[groupNamePrefix + "_NextBooking"]),
                TotalBookingsFifteenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_15minTotalBookings"]),
                TotalBookingsFifteenMinHDP = Converter.ToDecimal(reader[groupNamePrefix + "_15minTotalBookingsHdp"]),
                TenMinOU = Converter.ToDecimal(reader[groupNamePrefix + "_10minOU"]),
                FifteenMinNoOfCorners = Converter.ToDecimal(reader[groupNamePrefix + "_15minCorners"]),
                SingleTeamOU = Converter.ToDecimal(reader[groupNamePrefix + "_SingleTeamOU"]),
                TenMinHdp = Converter.ToDecimal(reader[groupNamePrefix + "_10minHdp"]),
                TenMinOneXTwo = Converter.ToDecimal(reader[groupNamePrefix + "_10min1x2"])
            };

            return traderGroupInfo;
        }
    }
}