// -----------------------------------------------------------------------
// <copyright file="MetricGamingService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.MatchWinLoss2a;
    using Fanex.Common;
    using Fanex.Excel;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MetricGamingService : ServiceBase, IMetricGamingService
    {
        private const int MetricGamingByBettypeTotalLabelColumnIndex = 0;
        private const int MetricGamingByBettypeLineTotalColumnIndex = 3;
        private const int MetricGamingByBettypeTurnoverColumnIndex = 6;
        private const int MetricGamingByBettypeMarginColumnIndex = 7;
        private readonly MatchWinLoss2aUtilityService _matchWinLoss2aUtilityService;
        private string _matchWinLoss2aCacheMetricGamingDataKey = "Fanex.AccountingReports.Main.MatchWinLoss2a.MetricGaming" +
        (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);
        
        private Dictionary<object, string> _cachedBettypesName;

        public MetricGamingService()
        {
            _matchWinLoss2aUtilityService = new MatchWinLoss2aUtilityService();
            _cachedBettypesName = new Dictionary<object, string>();
        }

        public GridOptions GetMetricGamingByBettypeReport(GetMetricGamingCriteria criteria)
        {
            var rawData = GetReportData<dynamic>(criteria);
            var bettypes = rawData.Select(x => x.bettype).Distinct().ToList();
            _matchWinLoss2aUtilityService.SaveBetTypeCacheRef(bettypes, ref _cachedBettypesName);
            var gridOptions = BuildMetricGamingByBettypeGridOptions(rawData.ToList(), criteria);
            SessionManager.Set<GridOptions>(_matchWinLoss2aCacheMetricGamingDataKey, gridOptions);
            return gridOptions;
        }

        public IWorkbook GetMetricGamingByBettypeReportWorkboook(GetMetricGamingCriteria criteria)
        {
            var report = SessionManager.Get<GridOptions>(_matchWinLoss2aCacheMetricGamingDataKey);
            if (report == null)
            {
                report = GetMetricGamingByBettypeReport(criteria);
            }

            FanexExcel fanexExcel = new FanexExcel(report);
            IWorkbook workbook = fanexExcel.ToExcel2003(MainResources.MatchWinLoss2a_SheetName);
            return workbook;
        }

        public string GetMetricGamingByBetypeReportName(GetMetricGamingCriteria criteria)
        {
            return string.Format(
                MainResources.MatchWinLoss2aDetailByBetType_ReportName,
                criteria.WinlostDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
        }

        private GridOptions InitMetricGamingByBettypeGridOptions(GetMetricGamingCriteria criteria)
        {
            return new GridOptions
            {
                Columns = MetricGamingByBettypeHeader(),
                ShowFooter = true,
                ShowTitle = true,
                ShowDistinctRow = false,
                ShowHighlightRow = false,
                TitleInfoText = string.Format(
                "Metric Gaming - Superlive Win/Loss Report for {0}",
                criteria.WinlostDate.ToShortFormatedDate()),
                TitleType = TitleType.TwoLineBlue
            };
        }

        private List<HeaderColumns> MetricGamingByBettypeHeader()
        {
            List<HeaderColumns> header = new List<HeaderColumns>();
            header.Add(new HeaderColumns() { HeaderText = Labels.Bet_Type, Name = Labels.Bet_Type, Width = 180, CellAlignment = HAlignment.Left });
            header.Add(new HeaderColumns() { HeaderText = Labels.Comm, Name = Labels.Comm, Width = 100, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Virtual_Comm, Name = Labels.Virtual_Comm, Width = 80, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Line_Total, Name = Labels.Line_Total, Width = 100, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Virtual_Line_Total, Name = Labels.Virtual_Line_Total, Width = 80, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Variance, Name = Labels.Variance, Width = 80, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Turnover, Name = Labels.Turnover, Width = 100, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            header.Add(new HeaderColumns() { HeaderText = Labels.Margin_Percent, Name = Labels.Margin_Percent, Width = 80, CellAlignment = HAlignment.Right, CustomCellType = ExcelCellType.Double });
            return header;
        }

        private GridOptions BuildMetricGamingByBettypeGridOptions(List<dynamic> rawData, GetMetricGamingCriteria criteria)
        {
            var gridOptions = InitMetricGamingByBettypeGridOptions(criteria);
            List<GridDataRow> dataRows = new List<GridDataRow>();
            GridDataRow totalDataRow = null;
            int dataRowIndex = 0;
            foreach (var bettype in rawData)
            {
                GridDataRow dataRow = BuildMetricGamingByBettypeDataRow(bettype);
                dataRows.Add(dataRow);
                if (dataRowIndex == 0)
                {
                    totalDataRow = dataRow.Clone();
                }
                else
                {
                    totalDataRow.AddValue(dataRow);
                }

                dataRowIndex++;
            }

            gridOptions.BodyRows = dataRows;
            if (totalDataRow != null)
            {
                totalDataRow = ReBuildMetricGamingByBettypeTotalRow(totalDataRow);
                gridOptions.FooterRows.Add(totalDataRow);
            }

            return gridOptions;
        }

        private GridDataRow BuildMetricGamingByBettypeDataRow(dynamic bettypeDataRow)
        {
            GridDataRow dataRow = new GridDataRow();
            decimal lineTotal = Converter.ToDecimal(bettypeDataRow.LineTotal);
            decimal virtualLineTotal = Converter.ToDecimal(bettypeDataRow.VirtualLineTotal);
            decimal variance = lineTotal - virtualLineTotal;
            decimal turnover = Converter.ToDecimal(bettypeDataRow.Turnover);
            bool liveIndicator = Converter.ToBool(bettypeDataRow.liveindicator);
            string liveIndicatorTitle = liveIndicator ? "!Live -" : string.Empty;
            dataRow.CreateColumns(
                new Columns
                {
                    Name = Labels.Bet_Type,
                    Value = $"{liveIndicatorTitle} {_cachedBettypesName[bettypeDataRow.bettype]}",
                    CustomAttributes = $"{liveIndicator},{bettypeDataRow.bettype}"});
            dataRow.CreateColumns(new Columns { Name = Labels.Comm, Value = Converter.ToDecimal(bettypeDataRow.comm) });
            dataRow.CreateColumns(new Columns { Name = Labels.Virtual_Comm, Value = Converter.ToDecimal(bettypeDataRow.VirtualCommission) });
            dataRow.CreateColumns(new Columns { Name = Labels.Line_Total, Value = lineTotal });
            dataRow.CreateColumns(new Columns { Name = Labels.Virtual_Line_Total, Value = virtualLineTotal});
            dataRow.CreateColumns(new Columns { Name = Labels.Variance, Value = variance });
            dataRow.CreateColumns(new Columns { Name = Labels.Turnover, Value = turnover });
            dataRow.CreateColumns(new Columns { Name = Labels.Margin_Percent, Value = turnover == 0 ? (object)Labels.NotAvailable : lineTotal / turnover * 100 });
            return dataRow;
        }

        private GridDataRow ReBuildMetricGamingByBettypeTotalRow(GridDataRow totalRow)
        {
            decimal lineTotal = Converter.ToDecimal(totalRow.Columns[MetricGamingByBettypeLineTotalColumnIndex].Value);
            decimal turnOver = Converter.ToDecimal(totalRow.Columns[MetricGamingByBettypeTurnoverColumnIndex].Value);
            totalRow.Columns[MetricGamingByBettypeTotalLabelColumnIndex].Value = "Metric Gaming - Superlive Total";
            totalRow.Columns[MetricGamingByBettypeMarginColumnIndex].Value = turnOver == 0 ? (object)Labels.NotAvailable : lineTotal / turnOver * 100;
            return totalRow;
        }
    }
}