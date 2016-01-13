// -----------------------------------------------------------------------
// <copyright file="ExportExcelHourlyService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.AccountingReports.Core.Web;
using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System.Collections.Generic;
    using System.Drawing;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;
    using Fanex.Common;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExportExcelHourlyService : IExportExcelHourlyService
    {
        private ICasinoGameStatisticService _service;

        public ExportExcelHourlyService()
            : this(null)
        {
        }

        public ExportExcelHourlyService(ICasinoGameStatisticService service)
        {
            _service = service ?? new CasinoGameStatisticService();
        }

        public void ExportReport(GetReportByHourlyCriteria criteria)
        {
            HSSFWorkbook excelFile = new HSSFWorkbook();

            string sheetName = "Hourly Report";
            ISheet worksheet = excelFile.CreateSheet(sheetName);
            int currentRow = 0;

            string key = Utilities.GetReportKey(criteria.ViewReport);
            var _reportRepository = new CasinoGameStatisticRepository();
            var data = SessionManager.Get<Hourly>(key) ?? _reportRepository.GetReportByHourly(criteria);

            InitColumn(ref worksheet);

            ExportReportTitle(excelFile, ref worksheet, ref currentRow, criteria, true);
            ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
            ExportBodyData(excelFile, ref worksheet, ref currentRow, data.DataAccumulatedList, criteria);
            ExportAverage(excelFile, ref worksheet, ref currentRow, data, criteria);
            currentRow++;

            ExportReportTitle(excelFile, ref worksheet, ref currentRow, criteria, false);
            ExportReportHeader(excelFile, ref worksheet, ref currentRow, criteria);
            ExportBodyData(excelFile, ref worksheet, ref currentRow, data.DataOnEachHourList, criteria);

            string fileName = criteria.Product.Replace(" ", "_") + "_Game_Statistic_Hourly_Report_On_" + criteria.FromDate.Replace(" ", "_").Replace(",", string.Empty);
            NpoiExcelExtensions.ResponseStreamExcel(excelFile, fileName);
        }

        private void ExportReportTitle(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetReportByHourlyCriteria criteria, bool isAccumulated)
        {
            IRow row = worksheet.CreateRow(currentRow);
            int currentColumn = 0;
            int endColumn = criteria.ShowRefundColumn ? 11 : 10;
            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
            string reportTitle = string.Empty;
            if (isAccumulated)
            {
                reportTitle = string.Format(GamingResources.CasinoGameStatistic_Accumulated_Title, criteria.Product, criteria.UserName, criteria.FromDate);
            }
            else
            {
                reportTitle = string.Format(GamingResources.CasinoGameStatistic_Hourly_Title, criteria.Product, criteria.UserName, criteria.FromDate);
            }

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, ++currentRow, endColumn, reportTitle, headerStyle);
            currentRow++;
        }

        private void ExportReportHeader(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, GetReportByHourlyCriteria criteria)
        {
            IRow row = worksheet.CreateRow(currentRow);

            ICellStyle headerStyle = NpoiExcelExtensions.HeaderStyle(excelFile);

            int currentColumn = 0;
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.Time, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.OnlinePlayers, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.BetCount, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.Currency, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.TotalBets, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.TotalWins, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.GameIncome, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.GamePayout, headerStyle);

            if (criteria.ShowRefundColumn)
            {
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.TotalRefund, headerStyle);
            }

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.CompanyTurnover, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.CompanyComm, headerStyle);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow + 1, currentColumn, GamingResources.CompanyNetIncome, headerStyle);

            currentRow++;
        }

        private void ExportBodyData(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, List<Hourly> data, GetReportByHourlyCriteria criteria)
        {
            currentRow++;

            ICellStyle textBoldRightStyle = NpoiExcelExtensions.CustomTextBackGroundCellStyle(excelFile, 220, 226, 188, HorizontalAlignment.Right);

            ICellStyle textCenterStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            ICellStyle decimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);            
            ICellStyle percentStyle = NpoiExcelExtensions.GetPercentageStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle numberStyle = NpoiExcelExtensions.IntStyle(excelFile, HSSFColor.White.Index, false);
            ICellStyle negativeDecimalNumberStyle = NpoiExcelExtensions.GetNegativeDecimalNumberStyle(excelFile, HSSFColor.White.Index, false);

            var gameIncomeHighLightedStyle = InitHighlightedDecimalStyle(excelFile);
            var gamePayoutHighLightedStyle = InitHiglightedPercentStyle(excelFile);
            var gamnePayoutRedColorStyle = InitRedColorPercentStyle(excelFile);

            foreach (Hourly hourly in data)
            {
                int currentColumn = 0;
                IRow row = worksheet.CreateRow(currentRow);
                double gamePayout = string.IsNullOrEmpty(hourly.GamePayout) ? 0 : Converter.To<double>(hourly.GamePayout);

                var gamePayoutStyle = percentStyle;
                if (gamePayout >= 1.05)
                {
                    gamePayoutStyle = gamePayoutHighLightedStyle;
                }
                else if (gamePayout > 1)
                {
                    gamePayoutStyle = gamnePayoutRedColorStyle;
                }
                else
                {
                    gamePayoutStyle = percentStyle;
                }

                double gameIncome = string.IsNullOrEmpty(hourly.GameIncome) ? 0 : Converter.To<double>(hourly.GameIncome);
                ICellStyle gameIncomeStyle;
                if (gameIncome <= -100000)
                {
                    gameIncomeStyle = gameIncomeHighLightedStyle;
                }
                else if (gameIncome < 0)
                {
                    gameIncomeStyle = negativeDecimalNumberStyle;
                }
                else
                {
                    gameIncomeStyle = decimalStyle;
                }

                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, hourly.Name, textBoldRightStyle);
                
                if (string.IsNullOrEmpty(hourly.NumberOfGames))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, numberStyle);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, numberStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<int>(hourly.OnlinePlayers), numberStyle);
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<int>(hourly.NumberOfGames), numberStyle);
                }
                
                NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, hourly.Currency, textCenterStyle);

                if (string.IsNullOrEmpty(hourly.TotalBets))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.TotalBets), negativeDecimalNumberStyle);
                }

                if (string.IsNullOrEmpty(hourly.TotalWins))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.TotalWins), negativeDecimalNumberStyle);
                }

                if (string.IsNullOrEmpty(hourly.GameIncome))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, gameIncomeStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.GameIncome), gameIncomeStyle);
                }

                if (string.IsNullOrEmpty(hourly.GamePayout))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, gamePayoutStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.GamePayout), gamePayoutStyle);
                }
                
                if (criteria.ShowRefundColumn)
                {
                    if (string.IsNullOrEmpty(hourly.TotalRefund))
                    {
                        NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
                    }
                    else
                    {
                        NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.TotalRefund), decimalStyle);
                    }                    
                }

                if (string.IsNullOrEmpty(hourly.CompanyTO))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, decimalStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.CompanyTO), decimalStyle);
                }

                if (string.IsNullOrEmpty(hourly.CompanyComm))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, negativeDecimalNumberStyle);
                }
                else
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.CompanyComm), negativeDecimalNumberStyle);
                }

                if (string.IsNullOrEmpty(hourly.CompanyNetInCome))
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, string.Empty, negativeDecimalNumberStyle);
                }
                else 
                {
                    NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<double>(hourly.CompanyNetInCome), negativeDecimalNumberStyle);
                }

                currentRow++;
            }
        }

        private void ExportAverage(HSSFWorkbook excelFile, ref ISheet worksheet, ref int currentRow, Hourly hourly, GetReportByHourlyCriteria criteria)
        {
            NpoiSetting setting = new NpoiSetting()
            {
                Alignment = HorizontalAlignment.Right,
                BackgroundColor = Color.FromArgb(250, 151, 143),
                DataFormat = NpoiExcelExtensions.TwoDecimalDigitNumber,
                FillForegroundColorIndex = 55,
                FontColorIndex = HSSFColor.Black.Index,
                IsBold = true
            };

            int currentColumn = 0;
            int endColumn = criteria.ShowRefundColumn ? 9 : 8;

            ICellStyle style = NpoiExcelExtensions.CustomCellStyle(excelFile, setting);

            IRow row = worksheet.CreateRow(currentRow);

            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, GamingResources.Average, style);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn, Converter.To<decimal>(hourly.OnlinePlayers), style);
            NpoiExcelExtensions.RenderCell(ref worksheet, ref row, currentRow, ref currentColumn, currentRow, currentColumn + endColumn, string.Empty, style);

            currentRow++;
        }

        private ICellStyle InitHighlightedDecimalStyle(HSSFWorkbook excelFile)
        {
            NpoiSetting setting = new NpoiSetting()
            {
                Alignment = HorizontalAlignment.Right,
                BackgroundColor = Color.FromArgb(255, 255, 153),
                DataFormat = NpoiExcelExtensions.TwoDecimalDigitNumber,
                FillForegroundColorIndex = 54,
                FontColorIndex = HSSFColor.Red.Index,
                IsBold = false
            };

            ICellStyle style = NpoiExcelExtensions.CustomCellStyle(excelFile, setting);

            return style;
        }

        private ICellStyle InitHiglightedPercentStyle(HSSFWorkbook excelFile)
        {
            NpoiSetting setting = new NpoiSetting()
            {
                Alignment = HorizontalAlignment.Right,
                BackgroundColor = Color.FromArgb(255, 255, 153),
                DataFormat = NpoiExcelExtensions.NormalPercentFormat,
                FillForegroundColorIndex = 53,
                FontColorIndex = HSSFColor.Red.Index,
                IsBold = false
            };

            ICellStyle style = NpoiExcelExtensions.CustomCellStyle(excelFile, setting);

            return style;
        }

        private ICellStyle InitRedColorPercentStyle(HSSFWorkbook excelFile)
        {
            NpoiSetting setting = new NpoiSetting()
            {
                Alignment = HorizontalAlignment.Right,
                BackgroundColor = Color.White,
                DataFormat = NpoiExcelExtensions.NormalPercentFormat,
                FillForegroundColorIndex = 52,
                FontColorIndex = HSSFColor.Red.Index,
                IsBold = false
            };

            ICellStyle style = NpoiExcelExtensions.CustomCellStyle(excelFile, setting);

            return style;
        }

        private void InitColumn(ref ISheet worksheet)
        {
            for (int i = 0; i < 12; i++)
            {
                worksheet.SetColumnWidth(i, 150 * NpoiExcelExtensions.FontWidth);
            }
        }
    }
}