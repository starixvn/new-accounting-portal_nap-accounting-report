namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class DailyWinLoss2hService
    {
        private const string KenoReportKey = "Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h.Keno";

        private string _liveCasinoByBettypeReportKey = "Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h.LiveCasinoByBettypeReport" +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _mainLiveCasinoReportKey = "Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h.MainLiveCasinoReport" +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private string _liveCasinoWinLossByProductReportKey = "Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2h.LiveCasinoWinLossByProduct" +
           (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The live casino report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    },
                    EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.LiveCasino)
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var betTypeList = rawData.Select(data => Converter.ToInt(data.bettype))
                                             .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawData.GroupBy(data => data.bettype);
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit
                            }
                        };

                        betTypeWinLoss.Name = this.GetBetTypeNameFromWinLossDetail(betTypeWinLoss);
                        betTypeWinLoss.GrandTotal.Id = betTypeWinLoss.Id;
                        betTypeWinLoss.GrandTotal.Name = betTypeWinLoss.Name;
                        foreach (var data in betTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.bettype),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.site)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    betTypeWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    betTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.CreditTotal);
                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.DepositTotal);
                        report.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                        report.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                        betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                        report.ChildsInfo.Add(betTypeWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hLiveCasinoReportSessionKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(MainConstants.DailyWinLoss2hLiveCasinoReportSessionKey, null);
                throw;
            }
        }

        public WinLossInfo GetMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            WinLossInfo winLosses = InitWinLoss();
            winLosses.GrandTotal.Name = Labels.All_Total;
            winLosses.EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.LiveCasino);
            var rawData = this._reportRepository.GetMainLiveCasinoReport(criteria);
            SessionManager.Set<ProductWinLoss>(_liveCasinoByBettypeReportKey, rawData);
            var sportGroups = rawData.ReportRows.GroupBy(x => x.ProductId);
            foreach (var sportGroup in sportGroups)
            {
                var winLossBySport = InitWinLoss();
                int sportId = Converter.ToInt(sportGroup.Key);
                winLossBySport.Id = sportId;
                winLossBySport.CreditTotal.Id = sportId;
                winLossBySport.DepositTotal.Id = sportId;
                winLossBySport.GrandTotal.Id = sportId;
                winLossBySport.GrandTotal.Name = _dailyWinLoss2hUtilityService.GetSportName(sportId);
                var sportBySiteGroups = sportGroup.GroupBy(x => x.Site);
                foreach (var sportBySiteGroup in sportBySiteGroups)
                {
                    foreach (var data in sportBySiteGroup)
                    {
                        int siteType = Converter.ToInt(sportBySiteGroup.Key);
                        var winLoss = MappingWinLossData(data, siteType);
                        winLossBySport.GrandTotal.Add(winLoss);
                        winLosses.GrandTotal.Add(winLoss);

                        if (winLoss.SiteType == SiteType.Credit)
                        {
                            winLossBySport.CreditTotal.Add(winLoss);
                            winLosses.CreditTotal.Add(winLoss);
                        }
                        else if (winLoss.SiteType == SiteType.Deposit)
                        {
                            winLossBySport.DepositTotal.Add(winLoss);
                            winLosses.DepositTotal.Add(winLoss);
                        }
                    }
                }

                winLosses.ChildsInfo.Add(winLossBySport);
            }

            SessionManager.Set<WinLossInfo>(_mainLiveCasinoReportKey, winLosses);
            return winLosses;
        }

        public WinLossInfo GetLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria)
        {
            WinLossInfo winLosses = InitWinLoss();
            winLosses.GrandTotal.Name = Labels.All_Total;

            var rawData = SessionManager.Get<ProductWinLoss>(_liveCasinoByBettypeReportKey);
            if (rawData == null)
            {
                var getLiveCasinoReportCriteria = new GetLiveCasinoReportCriteria
                {
                    Winlostdate = criteria.Winlostdate
                };
                rawData = this._reportRepository.GetMainLiveCasinoReport(getLiveCasinoReportCriteria);
                SessionManager.Set<ProductWinLoss>(_liveCasinoByBettypeReportKey, rawData);
            }

            var rawDataBySport = rawData.ReportRows.Where(x => x.ProductId == criteria.SportId).ToList();
            var betTypeList = rawDataBySport.Select(data => Converter.ToInt(data.Bettype)).ToList();
            CacheRef.SaveCacheRefResponse(betTypeList.Cast<object>().ToList(), CacheRef.GetBettype, ref this._betTypeNameCache);
            var betTypeGroups = rawDataBySport.GroupBy(data => data.Bettype);
            foreach (var bettypeGroup in betTypeGroups)
            {
                var winLossByBettype = InitWinLoss();
                int bettypeId = Converter.ToInt(bettypeGroup.Key);
                winLossByBettype.Id = bettypeId;
                winLossByBettype.Name = _betTypeNameCache[bettypeId];
                winLossByBettype.GrandTotal.Name = _betTypeNameCache[bettypeId];
                foreach (var data in bettypeGroup)
                {
                    int siteType = Converter.ToInt(data.Site);
                    var winLoss = MappingWinLossData(data, siteType);
                    winLossByBettype.GrandTotal.Add(winLoss);
                    winLosses.GrandTotal.Add(winLoss);

                    if (winLoss.SiteType == SiteType.Credit)
                    {
                        winLossByBettype.CreditTotal.Add(winLoss);
                        winLosses.CreditTotal.Add(winLoss);
                    }
                    else if (winLoss.SiteType == SiteType.Deposit)
                    {
                        winLossByBettype.DepositTotal.Add(winLoss);
                        winLosses.DepositTotal.Add(winLoss);
                    }
                }

                winLosses.ChildsInfo.Add(winLossByBettype);
            }
            
            return winLosses;
        }

        /// <summary>
        /// Gets the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The live casino report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetKenoReport(GetKenoReportCriteria criteria)
        {
            try
            {
                WinLossInfo report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo
                    {
                        Name = Labels.All_Total
                    },
                    ChildsInfo = new List<WinLossInfo>(),
                    CreditTotal = new WinLossInfo
                    {
                        Name = Labels.Credit
                    },
                    DepositTotal = new WinLossInfo
                    {
                        Name = Labels.Deposit
                    },
                    EnableVirtual = _dailyWinLoss2hUtilityService.IsApplyVirtualRate(SportContants.Keno)
                };

                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData != null)
                {
                    var betTypeList = rawData.Select(data => Converter.ToInt(data.bettype))
                                             .ToList();
                    CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);
                    var betTypeGroups = rawData.GroupBy(data => data.bettype);
                    foreach (var betTypeGroup in betTypeGroups)
                    {
                        var betTypeWinLoss = new WinLossInfo
                        {
                            Id = Converter.ToInt(betTypeGroup.Key),
                            ChildsInfo = new List<WinLossInfo>(),
                            GrandTotal = new WinLossInfo(),
                            CreditTotal = new WinLossInfo
                            {
                                Name = Labels.Credit
                            },
                            DepositTotal = new WinLossInfo
                            {
                                Name = Labels.Deposit
                            }
                        };

                        betTypeWinLoss.Name = this.GetBetTypeNameFromWinLossDetail(betTypeWinLoss);
                        betTypeWinLoss.GrandTotal.Id = betTypeWinLoss.Id;
                        betTypeWinLoss.GrandTotal.Name = betTypeWinLoss.Name;
                        foreach (var data in betTypeGroup)
                        {
                            WinLossInfo info = new WinLossInfo
                            {
                                Id = Converter.ToInt(data.bettype),
                                WinLoss = Converter.ToDecimal(data.WinLost),
                                Position = Converter.ToDecimal(data.Position),
                                Commission = Converter.ToDecimal(data.Commission),
                                LineTotal = Converter.ToDecimal(data.LineTotal),
                                Turnover = Converter.ToDecimal(data.Turnover),
                                SiteType = Converter.ToInt(data.site)
                            };

                            var rowType = _dailyWinLoss2hUtilityService.GetRowType(0, info.SiteType);
                            switch (rowType)
                            {
                                case RowType._2hReportCredit:
                                    info.AliasName = Labels.Credit;
                                    betTypeWinLoss.CreditTotal.Add(info);
                                    break;
                                case RowType._2hReportDeposit:
                                    info.AliasName = Labels.Deposit;
                                    betTypeWinLoss.DepositTotal.Add(info);
                                    break;
                            }
                        }

                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.CreditTotal);
                        betTypeWinLoss.GrandTotal.Add(betTypeWinLoss.DepositTotal);
                        report.CreditTotal.Add(betTypeWinLoss.CreditTotal);
                        report.DepositTotal.Add(betTypeWinLoss.DepositTotal);
                        betTypeWinLoss = WinLossInfo.Format_Total(betTypeWinLoss);
                        report.ChildsInfo.Add(betTypeWinLoss);
                    }

                    report.ChildsInfo = report.ChildsInfo.OrderBy(sportGroup => sportGroup.Id).ToList();
                    report.GrandTotal.Add(report.CreditTotal);
                    report.GrandTotal.Add(report.DepositTotal);
                    report = WinLossInfo.Format_Total(report);
                }

                SessionManager.Set<WinLossInfo>(KenoReportKey, report);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                SessionManager.Set<WinLossInfo>(KenoReportKey, null);
                throw;
            }
        }

        /// <summary>
        /// Exports the live casino report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(MainConstants.DailyWinLoss2hLiveCasinoReportSessionKey);
                if (report == null)
                {
                    report = this.GetLiveCasinoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_LiveCasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// Header

                var title = MainResources.DailyWinLoss2h_LiveCasinoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                //// Header

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    SportContants.LiveCasino);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Exports the keno report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportKenoReport(GetKenoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(KenoReportKey);
                if (report == null)
                {
                    report = this.GetKenoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_LiveCasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// Header

                var title = MainResources.DailyWinLoss2h_KenoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                //// Header

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    SportContants.LiveCasino);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_mainLiveCasinoReportKey);
                if (report == null)
                {
                    report = this.GetMainLiveCasinoReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_LiveCasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 8;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// Header

                var title = MainResources.DailyWinLoss2h_LiveCasinoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                _dailyWinLoss2hUtilityService.ExportMainCasinoHeader(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn);

                //// Header

                _dailyWinLoss2hUtilityService.ExportMainCasinoContent(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public void ExportGetLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_liveCasinoWinLossByProductReportKey);
                if (report == null)
                {
                    report = this.GetLiveCasinoWinLossByProductReport(criteria);
                }

                //// ExcelTypes

                ExcelFile excelFile = new ExcelFile();
                ExcelWorksheetCollection worksheets = excelFile.Worksheets;
                string sheetName = MainResources.DailyWinLoss2h_LiveCasinoReport_SheetName;
                ExcelWorksheet workSheet = worksheets.Add(sheetName);

                int startRow = 0;
                int startColumn = 0;
                int maxColumnIndex = 6;
                var currentRow = startRow;

                //// ExcelTypes
                //// MakeExcelFile

                var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
                var currentColumn = startColumn;
                CellRange currentCellRange;

                //// Header

                var title = MainResources.DailyWinLoss2h_LiveCasinoReport_Title;
                currentColumn = startColumn;
                currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
                currentCellRange.Merged = true;
                currentCellRange.Value = title;
                currentCellRange.Style = headerTextStyle;
                currentRow += 2;

                workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);
                this.ExportHeaderColumns_SportReport_NotApplyVirtualRate(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    false);

                //// Header

                _dailyWinLoss2hUtilityService.ExportContent_SportReport(
                    ref workSheet,
                    ref currentRow,
                    startRow,
                    startColumn,
                    report,
                    SportContants.LiveCasino);

                //// MakeExcelFile

                var reportName = string.Format(
                    MainResources.DailyWinLoss2h_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                excelFile.ReponseStream(reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private WinLossInfo InitWinLoss()
        {
            WinLossInfo winLoss = new WinLossInfo
            {
                ChildsInfo = new List<WinLossInfo>(),
                GrandTotal = new WinLossInfo(),
                CreditTotal = new WinLossInfo() { Name = Labels.Credit },
                DepositTotal = new WinLossInfo() { Name = Labels.Deposit }
            };

            return winLoss;
        }

        private WinLossInfo MappingWinLossData(ProductWinLoss productWinLoss, int siteType)
        {
            var winLoss = new WinLossInfo
            {
                SiteType = siteType,
                WinLoss = Converter.ToDecimal(productWinLoss.WinLost),
                Position = Converter.ToDecimal(productWinLoss.Position),
                Commission = Converter.ToDecimal(productWinLoss.Commission),
                LineTotal = Converter.ToDecimal(productWinLoss.LineTotal),
                Turnover = Converter.ToDecimal(productWinLoss.Turnover),
                ExtraPositionTaking = Converter.ToDecimal(productWinLoss.ExtraPositionTaking)
            };

            return winLoss;
        }
    }
}
