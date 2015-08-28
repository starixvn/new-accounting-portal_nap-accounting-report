// --------------------------------------------------------------------------
// <copyright file="LiveCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.08">Created</change>
//   <change who="Ricky.Nguyen" date="2012.12.3">Update Sel Licasino WinLoss Report</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Core.Configurations;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.LiveCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.LiveCasinoWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoWinLossService : ServiceBase, ILiveCasinoWinLossService
    {
        /// <summary>
        /// Gets all licensees.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>All licensees.</returns>
        public List<Licensee> GetAllLicensees(GetLicenseeByResellerCriteria criteria)
        {
            var licensees = new List<Licensee>();
            try
            {
                criteria.ProductCode = ProductPrefixConstants.LiveCasino;
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        var licenseeId = row.LicenseeID;
                        //// Not Include Demo Licensee
                        if (licenseeId != 5)
                        {
                            var licensee = new Licensee
                            {
                                LicenseeId = licenseeId,
                                LicenseeName = row.LicenseeName,
                                ResellerId = row.ResellerID,
                                ResellerName = row.ResellerName,
                                ResellerType = row.ResellerType
                            };

                            licensees.Add(licensee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return licensees;
        }

        /// <summary>
        /// Gets the live casino win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The live casino win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public LiveCasinoWinLoss GetLiveCasinoWinLoss(GetLiveCasinoWinLossCriteria criteria)
        {
            var liveCasinoWinLoss = new LiveCasinoWinLoss();
            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    double grandTotalStarExpressRevenue = 0, grandTotalAmountDueTo = 0;

                    var licenseeConfigSection = LicenseeReportSection.Instance.LicenseesGroup;
                    if (licenseeConfigSection != null && licenseeConfigSection.Count > 0)
                    {
                        foreach (LicenseeReportElement resellerConfig in licenseeConfigSection)
                        {
                            List<dynamic> groupResellerByConfigFile = new List<dynamic>();
                            var resellerIds = resellerConfig.ResellerIds.Split(',');
                            foreach (var resellerId in resellerIds)
                            {
                                groupResellerByConfigFile.AddRange(rawData.Where(
                                   r => r.ResellerID.ToString() == resellerId));
                            }

                            groupResellerByConfigFile.GroupBy(reseller => new
                            {
                                reseller.DistributorsChargeRate,
                                reseller.StarExpressChargeRate,
                                reseller.OneworksChargeRate
                            })
                           .ToList()
                           .ForEach(groupReseller =>
                           {
                               //// Group Reseller

                               LiveCasinoWinLossByGroupResellers numberGameWinLossGroupByResellers = new LiveCasinoWinLossByGroupResellers();

                               string amountDueToOneWorksInfor = string.Format("{0:n0}% Star Express's revenue", groupReseller.Key.OneworksChargeRate * 100);
                               string distributorRevenueInfor = string.Format("{0:n0}% gross win", groupReseller.Key.DistributorsChargeRate * 100);
                               string starExpressRevenueInfor = string.Format("{0:n0}% distributor's revenue", groupReseller.Key.StarExpressChargeRate * 100);

                               groupReseller.GroupBy(reseller => reseller.ResellerID)
                                   .ToList()
                                   .ForEach(licensees =>
                                   {
                                       AssignLicensee(
                                           numberGameWinLossGroupByResellers,
                                           licensees,
                                           ref grandTotalStarExpressRevenue,
                                           ref grandTotalAmountDueTo,
                                           ref starExpressRevenueInfor,
                                           ref distributorRevenueInfor);
                                   });

                               numberGameWinLossGroupByResellers.RevenueInfo.AmountDueToOneWorks = amountDueToOneWorksInfor;
                               numberGameWinLossGroupByResellers.RevenueInfo.StarExpressRevenue = starExpressRevenueInfor;
                               numberGameWinLossGroupByResellers.RevenueInfo.DistributorRevenue = distributorRevenueInfor;

                               //// Group Reseller

                               liveCasinoWinLoss.ResellersWinLoss.Add(numberGameWinLossGroupByResellers);
                           });
                        }
                    }

                    liveCasinoWinLoss.GrandTotal = new LiveCasinoWinLossDetail()
                    {
                        StarExpressRevenue = grandTotalStarExpressRevenue.Format(2),
                        AmountDueToOneWorks = grandTotalAmountDueTo.Format(2)
                    };
                }

                SessionManager.Set<LiveCasinoWinLoss>(LicenseeConstants.LiveCasinoWinLossSessionKey, liveCasinoWinLoss);
                return liveCasinoWinLoss;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Exports the excel live casino win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportExcelLiveCasinoWinLossReport(GetLiveCasinoWinLossCriteria criteria)
        {
            try
            {
                //// local varialbles

                var startCol = 0;
                var startRow = 2;
                var reportName = LicenseeResources.LiveCasinoWL_ReportFileName;
                var excelFile = new ExcelFile();
                var getSession = SessionManager.Get<LiveCasinoWinLoss>(LicenseeConstants.LiveCasinoWinLossSessionKey);
                getSession = getSession ?? this.GetLiveCasinoWinLoss(criteria);
                var excelReport = excelFile.Worksheets.Add(string.Format("LiveCasinoWinLoss"));

                //// local varialbles

                //// header

                this.ExportExcelLiveCasinoWinLossReportAssignHeader(
                    startRow,
                    startCol,
                    ref excelReport,
                    criteria.Title);

                //// header

                //// content

                if (getSession != null && getSession.ResellersWinLoss != null && getSession.ResellersWinLoss.Count > 0)
                {
                    int nextRow = 1;
                    var licenseesWinLoss = getSession.ResellersWinLoss;
                    var licenseeIndex = 0;
                    var totalLicenseeGroup = licenseesWinLoss.Count;
                    foreach (LiveCasinoWinLossByGroupResellers group in licenseesWinLoss)
                    {
                        this.ExportExcelLiveCasinoWinLossReportAssignRevenueInfoRow(startRow, startCol, ref nextRow, ref excelReport, group.RevenueInfo);
                        foreach (ResellerLiveCasinoWinLoss licensee in group.Resellers)
                        {
                            this.ExportExcelLiveCasinoWinLossReportAssignTotalRow(startRow, startCol, ref nextRow, ref excelReport, licensee.WinLossTotal);
                            foreach (LiveCasinoWinLossDetail item in licensee.LicenseeWinLossDetail)
                            {
                                this.ExportExcelLiveCasinoWinLossReportAssignBody(startRow, startCol, ref nextRow, ref excelReport, item);
                            }
                        }

                        CellRange currentCellRange;
                        if (licenseeIndex < totalLicenseeGroup - 1)
                        {
                            currentCellRange = excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + 6);
                            currentCellRange.Merged = true;
                            currentCellRange.Style.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
                            currentCellRange.Value = string.Empty;
                            nextRow++;
                        }

                        licenseeIndex++;
                    }

                    this.ExportExcelLiveCasinoWinLossReportAssignGrandTotalRow(startRow, startCol, ref nextRow, ref excelReport, getSession.GrandTotal);
                }

                //// content

                //// response file

                excelFile.ReponseStream(
                      string.Format(
                      "{0}_From_{1}_To_{2}",
                      reportName,
                      criteria.FromDate.ToShortDateString(),
                      criteria.ToDate.ToShortDateString()));

                //// response file
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Gets the type of the game.
        /// </summary>
        /// <returns>The type of the game.</returns>
        public List<GameType> GetGameType()
        {
            var gameTypeCriteria = new GetGameTypeCriteria();
            var gameTypeList = new List<GameType>();
            try
            {
                var rawData = GetReportData<dynamic>(gameTypeCriteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        var gameType = new GameType { Id = Converter.ToInt(row.Bettype), Name = row.GametypeName.ToString() };
                        gameTypeList.Add(gameType);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return gameTypeList;
        }

        //// export excel - private functions

        /// <summary>
        /// Assigns the licensee.
        /// </summary>
        /// <param name="numberGameWinLossGroupByResellers">The number game win loss group by resellers.</param>
        /// <param name="licensees">The licensees.</param>
        /// <param name="grandTotalStarExpressRevenue">The grand total star express revenue.</param>
        /// <param name="grandTotalAmountDueTo">The grand total amount due to.</param>
        /// <param name="starExpressRevenueInfo">The star express revenue info.</param>
        /// <param name="distributorRevenueInfo">The distributor revenue info.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void AssignLicensee(
            LiveCasinoWinLossByGroupResellers numberGameWinLossGroupByResellers,
            IGrouping<dynamic, dynamic> licensees,
            ref double grandTotalStarExpressRevenue,
            ref double grandTotalAmountDueTo,
            ref string starExpressRevenueInfo,
            ref string distributorRevenueInfo)
        {
            double totalDistributorRevenue = 0,
                                 totalStarExpressRevenue = 0,
                                 totalAmountDueTo = 0,
                                 totalTurnover = 0,
                                 totalGrosWin = 0,
                                 totalTransaction = 0;
            double currentDistributorChargeRate = 0,
                   currentStarExpressChargeRate = 0,
                   currentOneWorksChargeRate = 0;
            var currentResellerType = 0;
            string currentResellerName = string.Empty,
                   currentLicenseeName = string.Empty;
            ResellerLiveCasinoWinLoss resellerNumberGameWinLoss = new ResellerLiveCasinoWinLoss();
            foreach (var licensee in licensees)
            {
                //// Add Licensee Detail Infor

                double currentDistributorRevenue = 0,
                       currentStarExpressRevenue = 0,
                       currentAmountDueTo = 0,
                       currentTurnover = 0,
                       currentGrosWin = 0,
                       currentTransaction = 0;

                currentDistributorChargeRate = (double)licensee.DistributorsChargeRate;
                currentStarExpressChargeRate = (double)licensee.StarExpressChargeRate;
                currentOneWorksChargeRate = (double)licensee.OneworksChargeRate;
                currentResellerType = (int)licensee.ResellerType;
                currentResellerName = licensee.ResellerName;
                currentDistributorRevenue = (double)licensee.DistributorRevenue;
                currentGrosWin = (double)licensee.GrossWinloss;
                currentStarExpressRevenue = (double)licensee.StarExpressRevenue;
                currentTransaction = (double)licensee.BetCount;
                currentTurnover = (double)licensee.Turnover;
                currentAmountDueTo = (double)licensee.OneworksRevenue;
                if (currentResellerType == 1)
                {
                    resellerNumberGameWinLoss.LicenseeWinLossDetail.Add(new LiveCasinoWinLossDetail()
                    {
                        Licensee = licensee.LicenseeName,
                        Turnover = currentTurnover.Format(2),
                        TotalTransaction = currentTransaction.Format(),
                        GrossWin = currentGrosWin.Format(2),
                        DistributorRevenue = currentDistributorRevenue.Format(2),
                        StarExpressRevenue = currentStarExpressRevenue.Format(2),
                        AmountDueToOneWorks = currentAmountDueTo.Format(2)
                    });
                }

                //// Add Licensee Detail Infor

                totalDistributorRevenue += currentDistributorRevenue;
                totalStarExpressRevenue += currentStarExpressRevenue;
                totalAmountDueTo += currentAmountDueTo;
                totalTurnover += currentTurnover;
                totalTransaction += currentTransaction;
                totalGrosWin += currentGrosWin;
            }

            //// Reseller Group

            var turnover = totalTurnover.Format(2);
            var totalTrans = totalTransaction.Format();
            var grosswin = totalGrosWin.Format(2);
            var totalDistributor = "-";

            if (currentResellerType == 1)
            {
                totalDistributor = totalDistributorRevenue.Format(2);
                turnover = string.Empty;
                totalTrans = string.Empty;
                grosswin = string.Empty;
            }
            else
            {
                starExpressRevenueInfo = string.Format("{0}% gross win", currentStarExpressChargeRate * 100);
                distributorRevenueInfo = string.Empty;
            }

            resellerNumberGameWinLoss.WinLossTotal = new LiveCasinoWinLossDetail()
            {
                ResellerType = currentResellerType,
                Licensee = currentResellerName,
                Turnover = turnover,
                TotalTransaction = totalTrans,
                GrossWin = grosswin,
                DistributorRevenue = totalDistributor,
                StarExpressRevenue = totalStarExpressRevenue.Format(2),
                AmountDueToOneWorks = totalAmountDueTo.Format(2)
            };

            //// Reseller Group

            numberGameWinLossGroupByResellers.Resellers.Add(resellerNumberGameWinLoss);

            //// Update Grand Total
            grandTotalStarExpressRevenue += totalStarExpressRevenue;
            grandTotalAmountDueTo += totalAmountDueTo;
        }

        /// <summary>
        /// Exports the excel live casino win loss report assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelLiveCasinoWinLossReportAssignHeader(int startRow, int startCol, ref ExcelWorksheet excelReport, string title)
        {
            var backgroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            backgroundHeaderFormat.Font.Weight = ExcelFont.BoldWeight;
            backgroundHeaderFormat.Font.Color = ExcelExtensions.OddRowColor;
            backgroundHeaderFormat.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            var nextCol = 0;

            excelReport.Rows[startRow].Height = 255 * 2;
            excelReport.Cells[startRow, startCol].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol].Value = Labels.Licensee;
            excelReport.Columns[startCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Turnover;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.TotalTransactions;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.GrossWin;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = LicenseeResources.DistributorRevenue;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = LicenseeResources.StarExpressRevenue;
            excelReport.Columns[startCol + nextCol].Width = 255 * 25;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = LicenseeResources.AmountDueONEWorks;
            excelReport.Columns[startCol + nextCol].Width = 255 * 25;

            excelReport.Rows[startRow - 2].Height = 255 * 2;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = backgroundHeaderFormat;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value = title;
        }

        /// <summary>
        /// Exports the excel live casino win loss report assign body.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="winLoss">The win loss.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportExcelLiveCasinoWinLossReportAssignBody(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            LiveCasinoWinLossDetail winLoss)
        {
            var backGroundColor = ExcelExtensions.TotalRowColor;
            var cellStyle = ExcelExtensions.GetCellTextStyle(backGroundColor, false);
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            cellStyle.Font.Weight = ExcelFont.BoldWeight;
            var numberCellStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backGroundColor, false, false, false);
            numberCellStyle.Font.Weight = ExcelFont.BoldWeight;
         
            var transactionStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backGroundColor, false, false, false);
            numberCellStyle.Font.Weight = ExcelFont.BoldWeight;
                     
            var nextCol = 0;
            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol].Value = winLoss.Licensee;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.Turnover, 2);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.TotalTransaction);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.GrossWin, 2);

            var currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol + 1];
            currentCell.Value = winLoss.DistributorRevenue.Trim() == "-" ?
                (object)winLoss.DistributorRevenue :
                (object)Converter.ToDecimal(winLoss.DistributorRevenue, 2);
            currentCell.Style = numberCellStyle;
            nextCol++;

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.StarExpressRevenue, 2);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.AmountDueToOneWorks, 2);
            nextRow++;
        }

        /// <summary>
        /// Exports the excel live casino win loss report assign total row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="winLoss">The win loss.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportExcelLiveCasinoWinLossReportAssignTotalRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            LiveCasinoWinLossDetail winLoss)
        {
            var backGroundColor = ExcelExtensions.OddRowColor;
            var cellStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backGroundColor, false, false, false);
            var transactionStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backGroundColor, false, false, false);
            var nextCol = 0;
            ExcelCell currentCell;

            currentCell = excelReport.Cells[startRow + nextRow, startCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentCell.Value = winLoss.Licensee;

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Value = winLoss.Turnover != string.Empty ? Converter.ToDecimal(winLoss.Turnover) : (object)winLoss.Turnover;

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = transactionStyle;
            currentCell.Value = winLoss.TotalTransaction != string.Empty ? Converter.ToDecimal(winLoss.TotalTransaction) : (object)winLoss.TotalTransaction;

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Value = winLoss.GrossWin != string.Empty ? Converter.ToDecimal(winLoss.GrossWin, 2) : (object)winLoss.GrossWin;

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            if (winLoss.DistributorRevenue.Trim() == "-")
            {
                currentCell.Value = (object)winLoss.DistributorRevenue;
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            }
            else
            {
                currentCell.Value = (object)Converter.ToDecimal(winLoss.DistributorRevenue, 2);
                currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            }

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Value = Converter.ToDecimal(winLoss.StarExpressRevenue, 2);

            nextCol++;
            currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Value = Converter.ToDecimal(winLoss.AmountDueToOneWorks, 2);
            nextRow++;
        }

        /// <summary>
        /// Exports the excel live casino win loss report assign revenue information row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelLiveCasinoWinLossReportAssignRevenueInfoRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            LiveCasinoWinLossDetail winLoss)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(
                ExcelExtensions.OddRowColor,
                true,
                HorizontalAlignmentStyle.Center,
                true,
                ExcelExtensions.RevenueLicenseeColor);
            var nextCol = 0;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 3)).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyle;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = string.Empty;

            var currentCell = excelReport.Cells[startRow + nextRow, startCol + nextCol + 1];
            currentCell.Value = winLoss.DistributorRevenue;
            currentCell.Style = cellStyle;
            nextCol++;

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.StarExpressRevenue;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.AmountDueToOneWorks;
            nextRow++;
        }

        /// <summary>
        /// Exports the excel live casino win loss report assign grand total row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelLiveCasinoWinLossReportAssignGrandTotalRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            LiveCasinoWinLossDetail winLoss)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(
                ExcelExtensions.GrandTotalRowColor,
                true,
                HorizontalAlignmentStyle.Right,
                false,
                Color.Black);

            cellStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);
            var nextCol = 0;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 4)).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyle;
            excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = "Total";

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.StarExpressRevenue, 2).Format(2);

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.AmountDueToOneWorks, 2).Format(2);
            nextRow++;
        }

        //// export excel - private functions
    }
}