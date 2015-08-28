// --------------------------------------------------------------------------
// <copyright file="NumberGameWinlossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
//   <change who="Ricky.Nguyen" date="2012.12.3">Update Code</change>
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
    using Fanex.AccountingReports.Licensee.Criteria.NumberGameWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.NumberGameWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossService : ServiceBase, INumberGameWinLossService
    {
        public NumberGameWinLossService(IDynamicRepository dynamicRepository = null) :
            base(dynamicRepository)
        {
        }

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
                criteria.ProductCode = ProductPrefixConstants.NumberGame;
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    foreach (var row in rawData)
                    {
                        var licenseeId = row.LicenseeID;

                        // Not Include Demo Licensee
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
                this.HandleException(ex);
            }

            return licensees;
        }

        /// <summary>
        /// Gets the number game win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The number game win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameWinLoss GetNumberGameWinLoss(GetNumberGameWinLossCriteria criteria)
        {
            var numberGameWinloss = new NumberGameWinLoss();

            try
            {
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    double grandTotalLupineRevenue = 0, grandTotalAmountDueTo = 0;

                    var licenseeConfigSection = LicenseeReportSection.Instance.LicenseesGroup;
                    if (licenseeConfigSection != null && licenseeConfigSection.Count > 0)
                    {
                        foreach (LicenseeReportElement resellerConfig in licenseeConfigSection)
                        {
                            List<dynamic> groupResellerByConfigFile = new List<dynamic>();
                            var resellerIds = resellerConfig.ResellerIds.Split(',');
                            //// Group Reseller  by Id from File Config, to get Order
                            foreach (var resellerId in resellerIds)
                            {
                                groupResellerByConfigFile.AddRange(rawData.Where(r => r.ResellerID.ToString() == resellerId));
                            }

                            // After Group by ID, then group Reseller by Charge Rate
                            groupResellerByConfigFile.GroupBy(reseller => new
                            {
                                reseller.DistributorsChargeRate,
                                reseller.LupineChargeRate,
                                reseller.OneworksChargeRate
                            })
                           .ToList()
                           .ForEach(groupReseller =>
                           {
                               //// Each Reseller Group, Build Information String of Charge Rate

                               //// Group Reseller

                               NumberGameWinLossGroupByResellers numberGameWinLossGroupByResellers = new NumberGameWinLossGroupByResellers();

                               string amountDueToOneWorksInfor = string.Format("{0:n0}% Lupine's revenue", groupReseller.Key.OneworksChargeRate * 100);
                               string distributorRevenueInfor = string.Format("{0:n0}% gross win", groupReseller.Key.DistributorsChargeRate * 100);
                               string lupineRevenueInfor = string.Format("{0:n0}% distributor's revenue", groupReseller.Key.LupineChargeRate * 100);

                               // Group Licensee by Reseller Id
                               groupReseller.GroupBy(reseller => reseller.ResellerID)
                                   .ToList()
                                   .ForEach(licensees =>
                                   {
                                       // With Each Licensee Group, Calculate Total, bind data
                                       AssignLicensee(
                                           numberGameWinLossGroupByResellers,
                                           licensees,
                                           ref grandTotalLupineRevenue,
                                           ref grandTotalAmountDueTo,
                                           ref lupineRevenueInfor,
                                           ref distributorRevenueInfor);
                                   });

                               numberGameWinLossGroupByResellers.RevenueInfo.AmountDueToOneWorks = amountDueToOneWorksInfor;
                               numberGameWinLossGroupByResellers.RevenueInfo.LupineRevenue = lupineRevenueInfor;
                               numberGameWinLossGroupByResellers.RevenueInfo.DistributorRevenue = distributorRevenueInfor;

                               //// Group Reseller

                               // Add group of Reseller to Parent
                               numberGameWinloss.LicenseesWinLoss.Add(numberGameWinLossGroupByResellers);
                           });
                        }
                    }

                    numberGameWinloss.GrandTotal = new LicenseeNumberGameWinLoss()
                    {
                        LupineRevenue = grandTotalLupineRevenue.Format(2),
                        AmountDueToOneWorks = grandTotalAmountDueTo.Format(2)
                    };
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<NumberGameWinLoss>(LicenseeConstants.NumberGameWinLossSessionKey, numberGameWinloss);

            return numberGameWinloss;
        }

        /// <summary>
        /// Exports the excel number game win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportExcelNumberGameWinLossReport(GetNumberGameWinLossCriteria criteria)
        {
            try
            {
                //// local varialbles

                var startCol = 0;
                var startRow = 2;
                var reportName = LicenseeResources.NumberGameWL_ReportFileName;
                var excelFile = new ExcelFile();
                var getSessionData = SessionManager.Get<NumberGameWinLoss>(LicenseeConstants.NumberGameWinLossSessionKey);
                getSessionData = getSessionData ?? this.GetNumberGameWinLoss(criteria);
                var reportSheet = excelFile.Worksheets.Add(LicenseeResources.NumberGameWL_NumberGameWinloss);

                //// local varialbles

                //// header

                var excelFileName = string.Format(
                        "Number Game Win Loss Report from {0} to {1}",
                        criteria.FromDate.ToShortFormatedDate(),
                        criteria.ToDate.ToShortFormatedDate());

                this.ExportExcelNumberGameWinLossReportAssignHeader(
                    startRow,
                    startCol,
                    ref reportSheet,
                    excelFileName);

                //// header

                //// content

                if (getSessionData != null && getSessionData.LicenseesWinLoss != null && getSessionData.LicenseesWinLoss.Count > 0)
                {
                    int nextRow = 1;
                    var licenseesWinLoss = getSessionData.LicenseesWinLoss;
                    var licenseeIndex = 0;
                    var totalLicenseeGroup = licenseesWinLoss.Count;
                    CellRange currentCellRange;
                    foreach (NumberGameWinLossGroupByResellers group in licenseesWinLoss)
                    {
                        this.ExportExcelNumberGameWinLossReportAssignRevenueInfoRow(startRow, startCol, ref nextRow, ref reportSheet, group.RevenueInfo);
                        foreach (ResellerNumberGameWinLoss licensee in group.Resellers)
                        {
                            this.ExportExcelNumberGameWinLossReportAssignTotalRow(startRow, startCol, ref nextRow, ref reportSheet, licensee.WinLossTotal);
                            foreach (LicenseeNumberGameWinLoss item in licensee.LicenseeWinLossDetail)
                            {
                                this.ExportExcelNumberGameWinLossReportAssignBody(startRow, startCol, ref nextRow, ref reportSheet, item);
                            }
                        }

                        if (licenseeIndex < totalLicenseeGroup - 1)
                        {
                            currentCellRange = reportSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + 6);
                            currentCellRange.Merged = true;
                            currentCellRange.Style.Borders.SetBorders(MultipleBorders.Outside, Color.Gray, LineStyle.Thin);
                            currentCellRange.Value = string.Empty;
                            nextRow++;
                        }

                        licenseeIndex++;
                    }

                    this.ExportExcelNumberGameWinLossReportAssignGrandTotalRow(startRow, startCol, ref nextRow, ref reportSheet, getSessionData.GrandTotal);
                }

                //// content

                //// response file

                excelFile.ReponseStream(string.Format(
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
        /// Assigns the licensee.
        /// </summary>
        /// <param name="numberGameWinLossGroupByResellers">The number game win loss group by resellers.</param>
        /// <param name="licensees">The licensees.</param>
        /// <param name="grandTotalLupineRevenue">The grand total lupine revenue.</param>
        /// <param name="grandTotalAmountDueTo">The grand total amount due to.</param>
        /// <param name="lupineRevenueInfo">The lupine revenue information.</param>
        /// <param name="distributorRevenueInfo">The distributor revenue information.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void AssignLicensee(
            NumberGameWinLossGroupByResellers numberGameWinLossGroupByResellers,
            IGrouping<dynamic, dynamic> licensees,
            ref double grandTotalLupineRevenue,
            ref double grandTotalAmountDueTo,
            ref string lupineRevenueInfo,
            ref string distributorRevenueInfo)
        {
            double totalDistributorRevenue = 0,
                totalLupineRevenue = 0,
                totalAmountDueTo = 0,
                totalTurnover = 0,
                totalGrosWin = 0,
                totalTransaction = 0;
            double currentDistributorChargeRate = 0,
                currentLupineChargeRate = 0,
                currentOneWorksChargeRate = 0;
            var currentResellerType = 0;
            string currentResellerName = string.Empty,
                currentLicenseeName = string.Empty;
            ResellerNumberGameWinLoss resellerNumberGameWinLoss = new ResellerNumberGameWinLoss();
            //// Wich earch Licensee, calculate total of Reseller by Licensee
            foreach (var licensee in licensees)
            {
                //// Add Licensee Detail Infor

                double currentDistributorRevenue = 0,
                    currentLupineRevenue = 0,
                    currentAmountDueTo = 0,
                    currentTurnover = 0,
                    currentGrosWin = 0,
                    currentTransaction = 0;
                currentDistributorChargeRate = (double)licensee.DistributorsChargeRate;
                currentLupineChargeRate = (double)licensee.LupineChargeRate;
                currentOneWorksChargeRate = (double)licensee.OneworksChargeRate;
                currentResellerType = (int)licensee.ResellerType;
                currentResellerName = licensee.ResellerName;
                currentDistributorRevenue = (double)licensee.DistributorRevenue;
                currentGrosWin = (double)licensee.GrossWinloss;
                currentLupineRevenue = (double)licensee.LupineRevenue;
                currentTransaction = (double)licensee.BetCount;
                currentTurnover = (double)licensee.Turnover;
                currentAmountDueTo = (double)licensee.OneworksRevenue;
                //// If Reseller type == 1, mean Reseller has child, then add child licensse to Reseller
                if (currentResellerType == 1)
                {
                    resellerNumberGameWinLoss.LicenseeWinLossDetail.Add(new LicenseeNumberGameWinLoss()
                    {
                        Licensee = licensee.LicenseeName,
                        Turnover = currentTurnover.Format(2),
                        TotalTransaction = currentTransaction.Format(),
                        GrossWin = currentGrosWin.Format(2),
                        DistributorRevenue = currentDistributorRevenue.Format(2),
                        LupineRevenue = currentLupineRevenue.Format(2),
                        AmountDueToOneWorks = currentAmountDueTo.Format(2)
                    });
                }

                //// Add Licensee Detail Infor

                // Accumulate Total
                totalDistributorRevenue += currentDistributorRevenue;
                totalLupineRevenue += currentLupineRevenue;
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
                // If Reseller type not 1, mean has no child licensee, then remove DistributorRevernue information
                lupineRevenueInfo = string.Format("{0}% gross win", currentLupineChargeRate * 100);
                distributorRevenueInfo = string.Empty;
            }

            // Build Total Row of Reseller Group by Licensse
            resellerNumberGameWinLoss.WinLossTotal = new LicenseeNumberGameWinLoss()
            {
                ResellerType = currentResellerType,
                Licensee = currentResellerName,
                Turnover = turnover,
                TotalTransaction = totalTrans,
                GrossWin = grosswin,
                DistributorRevenue = totalDistributor,
                LupineRevenue = totalLupineRevenue.Format(2),
                AmountDueToOneWorks = totalAmountDueTo.Format(2)
            };

            //// Reseller Group

            numberGameWinLossGroupByResellers.Resellers.Add(resellerNumberGameWinLoss);

            // Update Grand Total
            grandTotalLupineRevenue += totalLupineRevenue;
            grandTotalAmountDueTo += totalAmountDueTo;
        }

        //// export excel - private functions

        /// <summary>
        /// Exports the excel number game win loss report assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelSheet">The excel sheet.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelNumberGameWinLossReportAssignHeader(int startRow, int startCol, ref ExcelWorksheet excelSheet, string title)
        {
            var headerStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            headerStyle.Font.Weight = ExcelFont.BoldWeight;
            headerStyle.Font.Color = ExcelExtensions.OddRowColor;
            headerStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            headerStyle.Font.Color = ExcelExtensions.OddRowColor;
            var nextCol = 0;

            excelSheet.Rows[startRow].Height = 255 * 2;
            excelSheet.Cells[startRow, startCol].Style = headerStyle;
            excelSheet.Cells[startRow, startCol].Value = Labels.Licensee;
            excelSheet.Columns[startCol].Width = 255 * 15;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = Labels.Turnover;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 20;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = Labels.TotalTransactions;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 15;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = Labels.GrossWin;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 15;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = LicenseeResources.DistributorRevenue;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 15;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = LicenseeResources.LupineRevenue;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 25;

            excelSheet.Cells[startRow, startCol + (nextCol += 1)].Style = headerStyle;
            excelSheet.Cells[startRow, startCol + nextCol].Value = LicenseeResources.AmountDueONEWorks;
            excelSheet.Columns[startCol + nextCol].Width = 255 * 25;

            excelSheet.Rows[startRow - 2].Height = 255 * 2;
            excelSheet.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            excelSheet.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = headerStyle;
            excelSheet.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value = title;
        }

        /// <summary>
        /// Exports the excel number game win loss report assign body.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelSheet">The excel sheet.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelNumberGameWinLossReportAssignBody(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelSheet,
            LicenseeNumberGameWinLoss winLoss)
        {
            Color backGroundColor = ExcelExtensions.TotalRowColor;
            var cellStyle = ExcelExtensions.GetCellTextStyle(backGroundColor, false);
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            var numberCellStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backGroundColor, false, false, false);
            var transactionStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backGroundColor, false, false, false);
            var nextCol = 0;
            excelSheet.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol].Value = winLoss.Licensee;
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.Turnover, 2);
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = transactionStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.TotalTransaction);
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.GrossWin, 2);
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.DistributorRevenue, 2);
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.LupineRevenue, 2);
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = numberCellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(winLoss.AmountDueToOneWorks, 2);
            nextRow++;
        }

        /// <summary>
        /// Exports the excel number game win loss report assign total row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelSheet">The excel sheet.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelNumberGameWinLossReportAssignTotalRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelSheet,
            LicenseeNumberGameWinLoss winLoss)
        {
            var backGroundColor = ExcelExtensions.OddRowColor;
            var cellStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, backGroundColor, false, false, false);
            var transactionStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, backGroundColor, false, false, false);
            var nextCol = 0;
            ExcelCell currentCell;

            currentCell = excelSheet.Cells[startRow + nextRow, startCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentCell.Value = winLoss.Licensee;

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Value = winLoss.Turnover != string.Empty ? Converter.ToDecimal(winLoss.Turnover) : (object)winLoss.Turnover;

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = transactionStyle;
            currentCell.Value = winLoss.TotalTransaction != string.Empty ? Converter.ToDecimal(winLoss.TotalTransaction) : (object)winLoss.TotalTransaction;

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Value = winLoss.GrossWin != string.Empty ? Converter.ToDecimal(winLoss.GrossWin, 2) : (object)winLoss.GrossWin;

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Value = winLoss.DistributorRevenue.Trim() == "-" ? (object)winLoss.DistributorRevenue : (object)Converter.ToDecimal(winLoss.DistributorRevenue, 2);

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Value = Converter.ToDecimal(winLoss.LupineRevenue, 2);

            nextCol++;
            currentCell = excelSheet.Cells[startRow + nextRow, startCol + nextCol];
            currentCell.Style = cellStyle;
            currentCell.Style.Font.Weight = ExcelFont.BoldWeight;
            currentCell.Value = Converter.ToDecimal(winLoss.AmountDueToOneWorks, 2);
            nextRow++;
        }

        /// <summary>
        /// Exports the excel number game win loss report assign revenue information row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelSheet">The excel sheet.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelNumberGameWinLossReportAssignRevenueInfoRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelSheet,
            LicenseeNumberGameWinLoss winLoss)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(
                ExcelExtensions.OddRowColor,
                true,
                HorizontalAlignmentStyle.Center,
                true,
                ExcelExtensions.RevenueLicenseeColor);
            var nextCol = 0;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 3)).Merged = true;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyle;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = string.Empty;
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.DistributorRevenue;
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.LupineRevenue;
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.AmountDueToOneWorks;
            nextRow++;
        }

        /// <summary>
        /// Exports the excel number game win loss report assign grand total row.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelSheet">The excel sheet.</param>
        /// <param name="winLoss">The win loss.</param>
        private void ExportExcelNumberGameWinLossReportAssignGrandTotalRow(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelSheet,
            LicenseeNumberGameWinLoss winLoss)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(
                ExcelExtensions.GrandTotalRowColor,
                true,
                HorizontalAlignmentStyle.Right,
                false,
                Color.Black);
            cellStyle.Borders.SetBorders(MultipleBorders.Top, ExcelExtensions.GrandTotalSeparatorColor, LineStyle.Medium);

            var nextCol = 0;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 4)).Merged = true;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyle;
            excelSheet.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = "Total";
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.LupineRevenue;
            excelSheet.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelSheet.Cells[startRow + nextRow, startCol + nextCol].Value = winLoss.AmountDueToOneWorks;
            nextRow++;
        }

        //// export excel - private functions
    }
}