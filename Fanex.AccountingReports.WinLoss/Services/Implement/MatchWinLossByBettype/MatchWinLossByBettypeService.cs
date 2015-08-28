// -----------------------------------------------------------------------
// <copyright file="MatchWinLossByBettypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.13">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Services.Implement.MatchWinLossByBettype
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Models.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossByBettype;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Match Win Loss By Bet Type Service.
    /// </summary>
    public class MatchWinLossByBettypeService : ServiceBase, IMatchWinLossByBettypeService
    {
        private string _matchWinLossByBettypeReportSessionKey = "Fanex.AccountingReports.WinLoss.MatchWinLossByBettype." +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the match win loss by bet type.
        /// </summary>
        /// <param name="getMatchWinLossByBettypeCriteria">The get match win loss by bet type criteria.</param>
        /// <returns>The report data.</returns>
        public MatchWinLossByBettype GetMatchWinLossByBettype(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria)
        {
            try
            {
                var result = new MatchWinLossByBettype();

                using (var db = new ObjectDb(getMatchWinLossByBettypeCriteria.GetSettingKey()))
                {
                    var param = getMatchWinLossByBettypeCriteria.GetSpParams();                    
                    var reader = db.ExecuteReader(param);

                    if (reader == null)
                    {
                        return result;
                    }

                    while (reader.Read())
                    {
                        int bettypeId = Converter.ToInt(reader["bettype"]);
                        int homeId = Converter.ToInt(reader["homeid"]);
                        int awayId = Converter.ToInt(reader["awayid"]);
                        string bettype = CacheRef.GetBettype(bettypeId);
                        string homeTeam = CacheRef.GetTeamName(homeId);
                        string awayTeam = CacheRef.GetTeamName(awayId);

                        var reportInfo = new MatchWinLossByBettype()
                        {
                            IsLive = Converter.ToBool(reader["liveindicator"]),
                            Bettype = bettype,
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            WinLoss = Converter.ToDecimal(reader["matchwinlost"])
                        };

                        result.CalculateTotal(reportInfo);
                        result.ReportRows.Add(reportInfo);
                    }
                }

                SessionManager.Set<MatchWinLossByBettype>(_matchWinLossByBettypeReportSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}