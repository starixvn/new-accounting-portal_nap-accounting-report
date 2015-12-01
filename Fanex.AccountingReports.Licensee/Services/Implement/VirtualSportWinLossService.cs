// --------------------------------------------------------------------------
// <copyright file="VirtualSportWinlossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.26">Created</change>
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
    using Fanex.AccountingReports.Licensee.Criteria.VirtualSportWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.VirtualSportWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSportWinLossService : ServiceBase, IVirtualSportWinLossService
    {
        public VirtualSportWinLossService(IDynamicRepository dynamicRepository = null) :
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
                criteria.ProductCode = ProductPrefixConstants.VirtualSport;
                criteria.ExcludeBA = true;
                criteria.ExcludeTest = true;
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
        /// Gets Sport type of virtual sport.
        /// </summary>
        /// <returns>List Licensee Site.</returns>
        public List<SportType> GetSportType()
        {
            List<SportType> sporttypes = new List<SportType>();

            try
            {
                GetVirtualSportBySporttypeCriteria criteria = new GetVirtualSportBySporttypeCriteria();

                var rawData = GetReportData<dynamic>(criteria);
                if (rawData == null)
                {
                    return sporttypes;
                }

                foreach (var row in rawData)
                {
                    sporttypes.Add(new SportType()
                    {
                        Id = row.Sporttype,
                        Name = row.SportName
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return sporttypes;
        }

        /// <summary>
        /// Gets the virtual sport win loss.
        /// </summary>
        /// <param name="virtualSportWinLossCriteria">The virtual sport win loss criteria.</param>
        /// <returns>The virtual sport win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
         "SP2101:MethodMustNotContainMoreLinesThan",
         Justification = "Reviewed. Suppression is OK here.")]
        public VirtualSportWinLossNEW SelVirtualSportWinLoss(GetVirtualSportWinLossCriteria virtualSportWinLossCriteria)
        {
            VirtualSportWinLossNEW winloss = new VirtualSportWinLossNEW();
            winloss.Rows = new List<VirtualSportWinLossNEW>();
            try
            {
                var rawData = GetReportData<dynamic>(virtualSportWinLossCriteria);
                if (rawData == null || rawData.Count() == 0)
                {
                    SessionManager.Set<VirtualSportWinLossNEW>(LicenseeConstants.VirtualSportsSessionKey, winloss);
                    return winloss;
                }

                var licenseeConfigSection = LicenseeReportSection.Instance.LicenseesGroup;
                bool isShowDistributorRevenue = false;
                //// group reseller
                foreach (LicenseeReportElement resellerConfig in licenseeConfigSection)
                {
                    var resellerids = resellerConfig.ResellerIds.Split(',');
                    List<dynamic> groupReseller = new List<dynamic>();

                    foreach (string resellerId in resellerids)
                    {
                        var dataByResellerId = rawData.Where(s => s.ResellerID == Converter.ToInt(resellerId)).ToList();
                        if (dataByResellerId.Count > 0)
                        {
                            foreach (var d in dataByResellerId)
                            {
                                groupReseller.Add(d);
                            }
                        }
                    }

                    ////assign revenue
                    if (groupReseller.Count > 0)
                    {
                        ////assgin data for group resellers don't have licensees
                        if (resellerConfig.ResellerIds != "1,2")
                        {
                            VirtualSportWinLossNEW revenue_of_group_reseller = new VirtualSportWinLossNEW();
                            winloss.Rows.Add(revenue_of_group_reseller);
                            this.AssignRevenue(revenue_of_group_reseller, groupReseller, false);

                            foreach (var d in groupReseller)
                            {
                                var reseller = new VirtualSportWinLossNEW();
                                this.AssignDataVirtualSportWinLoss(reseller, d, false, false);
                                reseller.Type = "Reseller";
                                winloss.Rows.Add(reseller);
                            }
                        }
                        else
                        {
                            ////assgin data for group resellers have licensees
                            foreach (string resellerId in resellerids)
                            {
                                var dataByResellerId = rawData.Where(s => s.ResellerID == Converter.ToInt(resellerId)).ToList();
                                if (dataByResellerId.Count == 0)
                                {
                                    continue;
                                }

                                var arr_oneworksChargeRate = dataByResellerId.Select(s => s.OneworksChargeRate).Distinct().ToList();
                                var arr_inspiredChargeRate = dataByResellerId.Select(s => s.InspiredChargeRate).Distinct().ToList();
                                var arr_rate = arr_oneworksChargeRate.Count > arr_inspiredChargeRate.Count ? arr_oneworksChargeRate : arr_inspiredChargeRate;

                                foreach (var rate in arr_rate)
                                {
                                    isShowDistributorRevenue = true;

                                    var groupLicenseeByRate = dataByResellerId.Where(s =>
                                        (arr_oneworksChargeRate.Count > arr_inspiredChargeRate.Count ?
                                        s.OneworksChargeRate :
                                        s.InspiredChargeRate) == rate).ToList();
                                    var revenue_of_group_licensee = new VirtualSportWinLossNEW();
                                    winloss.Rows.Add(revenue_of_group_licensee);
                                    this.AssignRevenue(revenue_of_group_licensee, groupLicenseeByRate, true);

                                    var reseller = new VirtualSportWinLossNEW();
                                    reseller.Rows = new List<VirtualSportWinLossNEW>();
                                    this.AssignDataVirtualSportWinLoss(reseller, groupLicenseeByRate, true);
                                    reseller.Type = "Reseller";
                                    winloss.Rows.Add(reseller);

                                    foreach (var d in groupLicenseeByRate)
                                    {
                                        var licensee = new VirtualSportWinLossNEW();
                                        this.AssignDataVirtualSportWinLoss(licensee, d, true, true);
                                        licensee.Type = "Licensee";
                                        reseller.Rows.Add(licensee);
                                    }
                                }
                            }
                        }
                    }
                }

                //// grand total
                this.AssignDataVirtualSportWinLoss(winloss, rawData, isShowDistributorRevenue);
                winloss.Name = "Total";
                winloss.Type = "GrandTotal";

                SessionManager.Set<VirtualSportWinLossNEW>(LicenseeConstants.VirtualSportsSessionKey, winloss);
                return winloss;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new VirtualSportWinLossNEW();
            }
        }

        /// <summary>
        /// Exports the excel virtual sports win loss report.
        /// </summary>
        /// <param name="virtualSportWinLossCriteria">The virtual sport win loss criteria.</param>
        public void ExportExcelVirtualSportsWinLossReport(GetVirtualSportWinLossCriteria virtualSportWinLossCriteria)
        {
            try
            {
                //// local varialbles

                var reportName = LicenseeResources.VirtualSports_ReportFileName;
                var excelFile = new ExcelFile();
                var getCache = SessionManager.Get<VirtualSportWinLossNEW>(LicenseeConstants.VirtualSportsSessionKey);
                getCache = getCache ?? this.SelVirtualSportWinLoss(virtualSportWinLossCriteria);
                var excelReport = excelFile.Worksheets.Add(string.Format("VirtualSportWinLoss"));

                //// local varialbles

                //// header

                var title = string.Format(
                    "Virtual Sports Win Loss Report from {0} to {1}",
                    virtualSportWinLossCriteria.FromDate.ToString("MMM d, yyyy"),
                    virtualSportWinLossCriteria.ToDate.ToString("MMM d, yyyy"));
                this.ExportExcelVirtualSportsWinLossReportAssignHeader(ref excelReport, title);

                //// header

                //// content

                if (getCache != null && getCache.Rows != null && getCache.Rows.Count > 0)
                {
                    this.ExportExcelVirtualSportsWinLossReportAssignBody(ref excelReport, getCache);
                }

                //// content

                //// response file

                excelFile.ReponseStream(string.Format(
                    "{0}_From_{1}_To_{2}",
                    reportName,
                    virtualSportWinLossCriteria.FromDate.ToShortDateString(),
                    virtualSportWinLossCriteria.ToDate.ToShortDateString()));

                //// response file
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Assigns the revenue.
        /// </summary>
        /// <param name="reseller">The reseller.</param>
        /// <param name="data">The data value.</param>
        /// <param name="isShowDistributorRevenue">If set to <c>true</c> [is show distributor revenue].</param>
        private void AssignRevenue(VirtualSportWinLossNEW reseller, IEnumerable<dynamic> data, bool isShowDistributorRevenue)
        {
            try
            {
                var oneworksChargeRate = data.FirstOrDefault().OneworksChargeRate;
                var inspiredChargeRate = data.FirstOrDefault().InspiredChargeRate;
                var distributorRevenue = data.FirstOrDefault().DistributorsChargeRate;

                reseller.OneworksIncome = ((double)oneworksChargeRate * 100).Format(0) + "% gross win";
                reseller.AmountInspired = ((double)inspiredChargeRate * 100).Format(0) + "% gross win";

                if (isShowDistributorRevenue)
                {
                    reseller.DistributorRevenue = ((double)distributorRevenue * 100).Format(0) + "% gross win";
                }

                reseller.Type = "Revenue";
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Assigns the data virtual sport win loss.
        /// </summary>
        /// <param name="objAssignData">The object assign data.</param>
        /// <param name="data">The data value.</param>
        /// <param name="isLicensee">If set to <c>true</c> [is licensee].</param>
        /// <param name="isShowDistributorRevenue">If set to <c>true</c> [is show distributor revenue].</param>
        private void AssignDataVirtualSportWinLoss(VirtualSportWinLossNEW objAssignData, dynamic data, bool isLicensee, bool isShowDistributorRevenue)
        {
            try
            {
                if (data == null)
                {
                    return;
                }

                objAssignData.Name = isLicensee ? data.LicenseeName : data.ResellerName;

                int betcount = Convert.ToInt32(data.BetCount);
                objAssignData.BetCount = betcount.Format(0);

                double turnover = (double)data.Turnover;
                objAssignData.Turnover = turnover.Format(2);

                double oneworksIncome = (double)data.OneworksIncome;
                objAssignData.OneworksIncome = oneworksIncome.Format(2);

                double grosswinloss = (double)data.GrossWinloss;
                objAssignData.GrossWinloss = grosswinloss.Format(2);

                double amtInspired = (double)data.AmtDueToInspired;
                objAssignData.AmountInspired = amtInspired.Format(2);

                double amtONEWorks = oneworksIncome - amtInspired;
                objAssignData.AmountOneWorks = amtONEWorks.Format(2);

                if (isShowDistributorRevenue)
                {
                    double distributorRevenue = (double)data.DistributorRevenue;
                    objAssignData.DistributorRevenue = distributorRevenue.Format(2);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Assigns the data virtual sport win loss.
        /// </summary>
        /// <param name="objAssignData">The object assign data.</param>
        /// <param name="data">The data value.</param>
        /// <param name="isShowDistributorRevenue">If set to <c>true</c> [is show distributor revenue].</param>
        private void AssignDataVirtualSportWinLoss(VirtualSportWinLossNEW objAssignData, IEnumerable<dynamic> data, bool isShowDistributorRevenue)
        {
            try
            {
                if (data == null && !data.Any())
                {
                    return;
                }

                int betcount = 0;
                double turnover = 0, oneworksIncome = 0, grosswinloss = 0, amtInspired = 0, amtONEWorks = 0, distributorRevenue = 0;
                foreach (var d in data)
                {
                    betcount += Convert.ToInt32(d.BetCount);
                    turnover += (double)d.Turnover;
                    oneworksIncome += (double)d.OneworksIncome;
                    grosswinloss += (double)d.GrossWinloss;
                    amtInspired += (double)d.AmtDueToInspired;
                    distributorRevenue += (double)d.DistributorRevenue;
                }

                amtONEWorks = oneworksIncome - amtInspired;

                objAssignData.Name = data.FirstOrDefault().ResellerName;
                objAssignData.BetCount = betcount.Format(0);
                objAssignData.Turnover = turnover.Format(2);
                objAssignData.OneworksIncome = oneworksIncome.Format(2);
                objAssignData.GrossWinloss = grosswinloss.Format(2);
                objAssignData.AmountInspired = amtInspired.Format(2);
                objAssignData.AmountOneWorks = amtONEWorks.Format(2);
                if (isShowDistributorRevenue)
                {
                    objAssignData.DistributorRevenue = distributorRevenue.Format(2);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        //// export excel - private functions

        /// <summary>
        /// Exports the excel virtual sports win loss report assign header.
        /// </summary>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelVirtualSportsWinLossReportAssignHeader(ref ExcelWorksheet excelReport, string title)
        {
            int startCol = 0;
            int startRow = 2;
            var backgroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(60, 94, 165));
            backgroundHeaderFormat.Font.Weight = ExcelFont.BoldWeight;
            backgroundHeaderFormat.Font.Color = Color.FromArgb(255, 255, 255);
            backgroundHeaderFormat.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            backgroundHeaderFormat.Font.Color = Color.FromArgb(255, 255, 255);
            var nextCol = 0;

            excelReport.Rows[startRow].Height = 255 * 2;
            excelReport.Cells[startRow, startCol].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol].Value = Labels.Licensee;
            excelReport.Columns[startCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Bet_Count;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Turnover;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.GrossWin;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.DistributorRevenue;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.ONEworkIncome;
            excelReport.Columns[startCol + nextCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.AmountDueToInspired;
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
        /// Exports the excel virtual sports win loss report assign body.
        /// </summary>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="virtualSportsWinLoss">The virtual sports win loss.</param>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportExcelVirtualSportsWinLossReportAssignBody(ref ExcelWorksheet excelReport, VirtualSportWinLossNEW virtualSportsWinLoss)
        {
            var cellStyleLicensee = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.TotalRowColor, false, false, false);
            cellStyleLicensee.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var cellStyleTotal = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, Color.FromArgb(255, 255, 255), false, false, false);
            cellStyleTotal.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var cellStyleGrantTotal = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, Color.FromArgb(220, 226, 188), false, false, false);
            cellStyleGrantTotal.Font.Weight = ExcelFont.BoldWeight;
            cellStyleGrantTotal.HorizontalAlignment = HorizontalAlignmentStyle.Right;

            var cellStyleRevenue = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, Color.FromArgb(255, 255, 255), false, false, false);
            cellStyleRevenue.Font.Weight = ExcelFont.BoldWeight;
            cellStyleRevenue.Font.Italic = true;
            cellStyleRevenue.Font.Color = Color.FromArgb(213, 132, 52);
            cellStyleRevenue.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            int startCol = 0;
            int startRow = 2;
            int nextRow = 1;
            var nextCol = 0;
            var count = virtualSportsWinLoss.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                nextCol = 0;
                var row = virtualSportsWinLoss.Rows[i];
                if (row.Type == "Revenue")
                {
                    if (i > 0)
                    {
                        excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 7)).Merged = true;
                        excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyleRevenue;
                        excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = string.Empty;
                        nextRow++;
                        nextCol = 0;
                    }

                    excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + (nextCol += 3)).Merged = true;
                    excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Style = cellStyleRevenue;
                    excelReport.Cells.GetSubrangeAbsolute(startRow + nextRow, startCol, startRow + nextRow, startCol + nextCol).Value = string.Empty;
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleRevenue;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = row.DistributorRevenue;
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleRevenue;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = row.OneworksIncome;
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleRevenue;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = row.AmountInspired;
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleRevenue;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = string.Empty;
                }
                else
                {
                    excelReport.Cells[startRow + nextRow, startCol].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                    excelReport.Cells[startRow + nextRow, startCol].Style.Font.Weight = ExcelFont.BoldWeight;
                    excelReport.Cells[startRow + nextRow, startCol].Value = row.Name;
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.BetCount);
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.Turnover);
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.GrossWinloss);
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;

                    if (string.IsNullOrEmpty(row.DistributorRevenue))
                    {
                        excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = "-";
                        excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    }
                    else
                    {
                        excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.DistributorRevenue);
                        excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.Font.Weight = ExcelFont.BoldWeight;
                    }

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.Font.Weight = ExcelFont.BoldWeight;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.OneworksIncome);
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.Font.Weight = ExcelFont.BoldWeight;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.AmountInspired);
                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleTotal;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.Font.Weight = ExcelFont.BoldWeight;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(row.AmountOneWorks);

                    if ((row.Rows != null) && row.Rows.Count > 0)
                    {
                        int countInner = row.Rows.Count;
                        for (int j = 0; j < countInner; j++)
                        {
                            nextRow++;
                            var rowInner = row.Rows[j];
                            nextCol = 0;
                            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol].Style.Font.Weight = ExcelFont.BoldWeight;
                            excelReport.Cells[startRow + nextRow, startCol].Value = rowInner.Name;
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.BetCount);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.Turnover);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.GrossWinloss);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.DistributorRevenue);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.OneworksIncome);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.AmountInspired);
                            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleLicensee;
                            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(rowInner.AmountOneWorks);
                        }
                    }
                }

                nextRow++;
            }

            nextCol = 0;
            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol].Value = Labels.Total;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.BetCount);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.Turnover);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.GrossWinloss);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;

            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.DistributorRevenue);

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.OneworksIncome);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.AmountInspired);
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleGrantTotal;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(virtualSportsWinLoss.AmountOneWorks);
        }

        //// export excel - private functions
    }
}