// --------------------------------------------------------------------------
// <copyright file="SpecialMatchAnalysisService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Common;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis;
    using Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis;
    using Fanex.AccountingReports.Special.Resources;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpecialMatchAnalysisService : ServiceBase, ISpecialMatchAnalysisService
    {
        /// <summary>
        /// Gets the sport league.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The sport league.</returns>
        public List<SportLeague> GetSportLeague(GetSportLeagueCriteria criteria)
        {
            List<SportLeague> sportLeagues = new List<SportLeague>();

            try
            {
                var data = GetReportData<dynamic>(criteria);

                var sports = data.Select(s => s.sporttype).Distinct().ToList();
                foreach (var sportId in sports)
                {
                    SportLeague sportLeague = new SportLeague();
                    sportLeague.Id = sportId;
                    sportLeague.Name = CacheRef.GetSportName(sportId);
                    sportLeague.Leagues = new List<League>();
                    sportLeagues.Add(sportLeague);

                    var leagues = data.Where(s => s.sporttype == sportId).Select(s => s.leagueid).ToList();
                    foreach (var leagueId in leagues)
                    {
                        League league = new League()
                        {
                            Id = leagueId,
                            Name = CacheRef.GetLeagueName(leagueId)
                        };

                        sportLeague.Leagues.Add(league);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return sportLeagues;
        }

        /// <summary>
        /// Gets the league match.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The league match.</returns>
        public List<LeagueMatch> GetLeagueMatch(GetLeageMatchCriteria criteria)
        {
            List<LeagueMatch> leagueMatches = new List<LeagueMatch>();
            try
            {
                var data = GetReportData<dynamic>(criteria);
                var leagues = data.Select(s => s.leagueid).Distinct().ToList();

                foreach (var leagueId in leagues)
                {
                    LeagueMatch leagueMatch = new LeagueMatch();
                    leagueMatch.Id = leagueId;
                    leagueMatch.Name = CacheRef.GetLeagueName(leagueId);
                    leagueMatches.Add(leagueMatch);

                    var matches = data.Where(s => s.leagueid == leagueId).ToList();
                    foreach (var match in matches)
                    {
                        leagueMatch.Matches.Add(new Match()
                        {
                            MatchId = match.matchid,
                            MatchCode = match.matchcode,
                            ShowTime = match.showtime,
                            Sport = new BaseModel()
                            {
                                Id = match.sporttype,
                                Name = CacheRef.GetSportName(match.sporttype)
                            },
                            Home = new BaseModel()
                            {
                                Id = match.homeid,
                                Name = CacheRef.GetTeamName(match.homeid)
                            },
                            Away = new BaseModel()
                            {
                                Id = match.awayid,
                                Name = CacheRef.GetTeamName(match.awayid)
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return leagueMatches;
        }

        /// <summary>
        /// Gets the type of the bet.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The type of the bet.</returns>
        public List<Bettype> GetBetType(GetBetTypeCriteria criteria)
        {
            List<Bettype> bettypes = new List<Bettype>();
            try
            {
                var data = GetReportData<dynamic>(criteria);
                foreach (var d in data)
                {
                    bettypes.Add(new Bettype()
                    {
                        Id = d.bettype,
                        Name = CacheRef.GetBettype(d.bettype)
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return bettypes;
        }

        /// <summary>
        /// Gets the match analysis.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The match analysis.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public MatchAnalysisView GetMatchAnalysis(GetMatchAnalysisCriteria criteria)
        {
            MatchAnalysisView matchAnalysis = new MatchAnalysisView();
            var currencies = ReportService.ExchangeCurrencies;

            try
            {
                var getMatchAnalysisCriteria = GetMatchAnalysisCriteria(criteria);
                var data = GetReportData<dynamic>(getMatchAnalysisCriteria);
                MatchAnalysis chargedCommission = null;
                foreach (var d in data)
                {
                    MatchAnalysis row = new MatchAnalysis()
                    {
                        SMAWL = ((double)d.SMAWinLost).Format(2),
                        SMATO = ((double)d.SMATurnover).Format(2),

                        HouseWL = ((double)d.HouseWinloss).Format(2),
                        HouseTO = ((double)d.HouseTurnover).Format(2),

                        MemberWL = ((double)d.MemberWinLost).Format(2),
                        MemberTO = ((double)d.MemberTurnover).Format(2),

                        BetCount = ((int)d.betcount).Format(0),
                    };

                    switch (criteria.Reporttype)
                    {
                        case ReportType.Currency:
                            row.Currency = currencies[d.currency];
                            row.Name = row.Currency;
                            break;

                        case ReportType.Bettype:
                            row.Id = d.bettype;
                            row.Name = CacheRef.GetBettype(d.bettype);
                            break;

                        case ReportType.Super:
                            row.Currency = currencies[d.Currency];
                            row.Id = d.srecommend;
                            row.Name = d.username;
                            break;

                        case ReportType.Member:
                            row.Currency = currencies[d.currency];
                            row.Name = d.username;
                            break;

                        case ReportType.SuperViewOnMaster:
                            row.Currency = currencies[d.Currency];
                            row.Id = d.mrecommend;
                            row.Name = d.username;
                            break;

                        case ReportType.MasterViewOnAgent:
                            row.Currency = currencies[d.Currency];
                            row.Id = d.recommend;
                            row.Name = d.username;
                            break;

                        case ReportType.AgentViewOnMember:
                            row.Currency = currencies[d.Currency];
                            row.Id = d.custid;
                            row.Name = d.username;
                            break;
                    }

                    int chargedComissionBetType = 500;
                    if (row.Id == chargedComissionBetType && criteria.Reporttype == ReportType.Bettype)
                    {
                        chargedCommission = row;
                        chargedCommission.Name = "Charged Commission";
                        chargedCommission.BetCount = "0";
                    }
                    else
                    {
                        matchAnalysis.Rows.Add(row);
                    }
                }

                var total = new MatchAnalysis()
                {
                    SMAWL = data.Sum(s => (double)s.SMAWinLost).Format(2),
                    SMATO = data.Sum(s => (double)s.SMATurnover).Format(2),

                    HouseWL = data.Sum(s => (double)s.HouseWinloss).Format(2),
                    HouseTO = data.Sum(s => (double)s.HouseTurnover).Format(2),

                    MemberWL = data.Sum(s => (double)s.MemberWinLost).Format(2),
                    MemberTO = data.Sum(s => (double)s.MemberTurnover).Format(2),

                    BetCount = data.Sum(s => (int)s.betcount).Format(0),
                    Name = Labels.Total
                };
                matchAnalysis.Total = new List<MatchAnalysis> { total };

                if (chargedCommission != null)
                {
                    matchAnalysis.Total.Add(chargedCommission);

                    var grandTotal = new MatchAnalysis
                    {
                        Name = "All Total",
                        SMAWL = total.SMAWL,
                        SMATO = total.SMATO,
                        HouseWL = total.HouseWL,
                        HouseTO = total.HouseTO,
                        MemberWL = total.MemberWL,
                        MemberTO = total.MemberTO,
                        BetCount = total.BetCount,
                    };

                    matchAnalysis.Total.Add(grandTotal);
                    matchAnalysis.Total.First().SMAWL = Converter.ToString2(Converter.ToDecimal(total.SMAWL)
                                   - Converter.ToDecimal(chargedCommission.SMAWL));
                    matchAnalysis.Total.First().SMATO = Converter.ToString2(Converter.ToDecimal(total.SMATO)
                                   - Converter.ToDecimal(chargedCommission.SMATO));

                    matchAnalysis.Total.First().HouseWL = Converter.ToString2(Converter.ToDecimal(total.HouseWL)
                                   - Converter.ToDecimal(chargedCommission.HouseWL));
                    matchAnalysis.Total.First().HouseTO = Converter.ToString2(Converter.ToDecimal(total.HouseTO)
                                   - Converter.ToDecimal(chargedCommission.HouseTO));

                    matchAnalysis.Total.First().MemberWL = Converter.ToString2(Converter.ToDecimal(total.MemberWL)
                                    - Converter.ToDecimal(chargedCommission.MemberWL));
                    matchAnalysis.Total.First().MemberTO = Converter.ToString2(Converter.ToDecimal(total.MemberTO)
                                    - Converter.ToDecimal(chargedCommission.MemberTO));

                    matchAnalysis.Total.First().BetCount = Converter.ToString2(Converter.ToDecimal(total.BetCount)
                                    - Converter.ToDecimal(chargedCommission.BetCount));
                }

                SessionManager.Set(SpecialConstants.MatchAnalysisSessionKey + GetReportTypeName(criteria.Reporttype), matchAnalysis);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return matchAnalysis;
        }

        /// <summary>
        /// Exports the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportReport(GetMatchAnalysisCriteria criteria)
        {
            try
            {
                var matchAnalysis = SessionManager.Get<MatchAnalysisView>(SpecialConstants.MatchAnalysisSessionKey + GetReportTypeName(criteria.Reporttype));
                if (matchAnalysis == null)
                {
                    matchAnalysis = GetMatchAnalysis(criteria);
                }

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = SpecialResources.MatchAnalysis_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                var currentRow = startRow;

                this.ExportReportHeader(ref workSheet, startRow, startColumn, ref currentRow, criteria);
                currentRow++;
                this.ExportReportContent(ref workSheet, currentRow, startColumn, ref currentRow, criteria, matchAnalysis);

                var reportName = string.Format(
                    SpecialResources.MatchAnalysis_SheetName.Replace(" ", "_"),
                    criteria.EventDate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Gets the bet status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The bet status.</returns>
        private string GetBetStatus(int id)
        {
            string status = Labels.All;
            switch (id)
            {
                case 0:
                    status = Labels.All;
                    break;

                case 112:
                    status = Labels.Won;
                    break;

                case 113:
                    status = Labels.Lose;
                    break;

                case 111:
                    status = Labels.Draw;
                    break;
            }

            return status;
        }

        /// <summary>
        /// Gets the live status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The live status.</returns>
        private string GetLiveStatus(int? id)
        {
            string status = Labels.All;
            id = id == null ? 2 : id;
            switch (id)
            {
                case 2:
                    status = Labels.All;
                    break;

                case 1:
                    status = Labels.Live;
                    break;

                case 0:
                    status = Labels.None_live;
                    break;
            }

            return status;
        }

        private string GetBettype(string betTypeIds)
        {
            List<string> bettypes = new List<string>();
            var betTypeIdList = betTypeIds.Split(',');
            foreach (string betTypeId in betTypeIdList)
            {
                bettypes.Add(CacheRef.GetBettype(betTypeId));
            }

            return string.Join(", ", bettypes);
        }

        /// <summary>
        /// Gets the header excel.
        /// </summary>
        /// <param name="reportType">Type of the report.</param>
        /// <param name="statusId">The status identifier.</param>
        /// <returns>The header excel.</returns>
        private string GetHeaderExcel(ReportType reportType, int statusId)
        {
            string firstColumn = string.Empty;
            switch (reportType)
            {
                case ReportType.Currency:
                    firstColumn = Labels.Currency;
                    break;

                case ReportType.Bettype:
                    firstColumn = Labels.Bet_Type;
                    break;

                case ReportType.Member:
                    switch (statusId)
                    {
                        case 112:
                            firstColumn = Labels.Top_100_Won_Member;
                            break;

                        case 113:
                            firstColumn = Labels.Top_100_Lose_Member;
                            break;
                    }

                    break;

                case ReportType.Super:
                    firstColumn = Labels.Super;
                    break;

                case ReportType.SuperViewOnMaster:
                    firstColumn = Labels.Master;
                    break;

                case ReportType.MasterViewOnAgent:
                    firstColumn = Labels.Agent;
                    break;

                case ReportType.AgentViewOnMember:
                    firstColumn = Labels.Member;
                    break;
            }

            return firstColumn;
        }

        /// <summary>
        /// Exports the report header.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetMatchAnalysisCriteria criteria)
        {
            ////init default value
            criteria.Home = criteria.Home ?? string.Empty;
            criteria.Away = criteria.Away ?? string.Empty;
            criteria.Bettypes = criteria.Bettypes ?? string.Empty;
            criteria.League = criteria.League ?? string.Empty;

            //// ExcelTypes
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;

            bool isAddMoreCurrencyColumn = !(criteria.Reporttype == ReportType.Currency || criteria.Reporttype == ReportType.Bettype);
            var colspan = isAddMoreCurrencyColumn ? 8 : 7;

            CellRange currentCellRange;

            //// ExcelTypes
            //// Filter

            currentColumn = startColumn;
            for (int colIndex = 0; colIndex <= colspan; colIndex++)
            {
                worksheet.Columns[startColumn + colIndex].Width = 140 * ExcelExtensions.FontWidth;
            }

            var title = SpecialResources.MatchAnalysis_SheetName + " (" + Labels.Base_on_RM + ")";

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + colspan);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow += 2;

            //// Filter
            string reportType = GetReportTypeName(criteria.Reporttype);
            string betStatus = GetBetStatus(criteria.StatusId);
            string liveStatus = GetLiveStatus(criteria.LiveIndicator);
            string bettype = GetBettype(criteria.Bettypes);

            var filterText = Labels.Report_by + ": " + reportType;
            filterText += "        " + Labels.Bet_Status + ": " + betStatus;
            filterText += "        " + Labels.Live_Status + ": " + liveStatus;
            filterText += "\n" + Labels.Bet_Type + ": " + bettype;

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + colspan);
            currentCellRange.Merged = true;
            currentCellRange.Value = filterText;
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + colspan);

            //// Match Info

            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Event_Date;
            currentCellRange.Style.FormatTitleCell(headerColor);

            currentColumn++;
            var endColumn = isAddMoreCurrencyColumn ? currentColumn + 3 : currentColumn + 2;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.League;
            currentCellRange.Style.FormatTitleCell(headerColor);

            currentColumn += endColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Event;
            currentCellRange.Style.FormatTitleCell(headerColor);

            currentRow++;
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = criteria.EventDate.ToShortDateString();
            currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

            currentColumn++;
            endColumn = isAddMoreCurrencyColumn ? currentColumn + 3 : currentColumn + 2;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, endColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = criteria.League;
            currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

            currentColumn += endColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = criteria.Home.Length > 0 && criteria.Away.Length > 0 ?
                criteria.Home + " -" + Labels.Symbol_Vs + "- " + criteria.Away :
                criteria.Home + " " + criteria.Away;
            currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);
            currentRow++;

            //// Filter

            worksheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + colspan);

            //// Title

            // First Row
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = GetHeaderExcel(criteria.Reporttype, criteria.StatusId);
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Turnover;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Member_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Bet_Count;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Turnover;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Company_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agency_Turnover;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Agency_Win_Loss;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            if (isAddMoreCurrencyColumn)
            {
                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = Labels.Currency;
                currentCellRange.Style.FormatTitleCell(headerColor);
                currentColumn++;
            }

            currentRow++;
            //// Title
        }

        /// <summary>
        /// Exports the content of the report.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="matchAnalysis">The match analysis.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            GetMatchAnalysisCriteria criteria,
            MatchAnalysisView matchAnalysis)
        {
            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            var currentColumn = startColumn;

            bool isAddMoreCurrencyColumn = !(criteria.Reporttype == ReportType.Currency || criteria.Reporttype == ReportType.Bettype);

            CellRange currentCellRange;
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);
            var twoDecimalNumberStyleNegative = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                true);

            try
            {
                //// Rows
                foreach (MatchAnalysis row in matchAnalysis.Rows)
                {
                    currentColumn = startColumn;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = row.Name;
                    currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.MemberTO);
                    currentCellRange.Style = twoDecimalNumberStyle;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.MemberWL);
                    currentCellRange.Style = twoDecimalNumberStyle;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.BetCount);
                    currentCellRange.Style = intStyle;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.HouseTO);
                    currentCellRange.Style = twoDecimalNumberStyle;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.HouseWL);
                    currentCellRange.Style = twoDecimalNumberStyleNegative;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.SMATO);
                    currentCellRange.Style = twoDecimalNumberStyle;

                    currentColumn++;
                    currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                    currentCellRange.Merged = true;
                    currentCellRange.Value = Convert.ToDecimal(row.SMAWL);
                    currentCellRange.Style = twoDecimalNumberStyle;

                    if (isAddMoreCurrencyColumn)
                    {
                        currentColumn++;
                        currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = row.Currency;
                        currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);
                    }

                    currentRow++;
                }

                //// Total
                foreach (var total in matchAnalysis.Total)
                {
                    ExportReportTotalContent(currentRow, startColumn, isAddMoreCurrencyColumn, worksheet, total);
                    currentRow++;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]

        private void ExportReportTotalContent(int currentRow, int startColumn, bool isAddMoreCurrencyColumn, ExcelWorksheet worksheet, MatchAnalysis matchAnalysis)
        {
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.GrandTotalRowColor, true, false, false);
            intStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.GrandTotalRowColor, true, false, false);
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var twoDecimalNumberStyleNegative = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                true);
            twoDecimalNumberStyleNegative.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var currentColumn = startColumn;
            var currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = matchAnalysis.Name;
            currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Center);
            currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.MemberTO);
            currentCellRange.Style = twoDecimalNumberStyle;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.MemberWL);
            currentCellRange.Style = twoDecimalNumberStyle;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.BetCount);
            currentCellRange.Style = intStyle;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.HouseTO);
            currentCellRange.Style = twoDecimalNumberStyle;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.HouseWL);
            currentCellRange.Style = twoDecimalNumberStyleNegative;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.SMATO);
            currentCellRange.Style = twoDecimalNumberStyle;

            currentColumn++;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Convert.ToDecimal(matchAnalysis.SMAWL);
            currentCellRange.Style = twoDecimalNumberStyle;

            if (isAddMoreCurrencyColumn)
            {
                currentColumn++;
                currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn);
                currentCellRange.Merged = true;
                currentCellRange.Value = string.Empty;
                currentCellRange.Style = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true, HorizontalAlignmentStyle.Center);
                currentCellRange.Style.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            }
        }

        private string GetReportTypeName(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.Bettype:
                    return Labels.Bet_Type;

                default:
                    return reportType.ToString();
            }
        }

        private CriteriaBase GetMatchAnalysisCriteria(GetMatchAnalysisCriteria criteria)
        {
            switch (criteria.Reporttype)
            {
                case ReportType.SuperViewOnMaster:
                case ReportType.MasterViewOnAgent:
                case ReportType.AgentViewOnMember:
                    return new GetMatchAnalysisByUserLevelCriteria()
                    {
                        EventDate = criteria.EventDate,
                        MatchId = criteria.MatchId,
                        LiveIndicator = criteria.LiveIndicator,
                        Bettypes = criteria.Bettypes,
                        StatusId = criteria.StatusId,
                        CustId = criteria.CustId,
                        Reporttype = criteria.Reporttype
                    };

                default:
                    return criteria;
            }
        }
    }
}