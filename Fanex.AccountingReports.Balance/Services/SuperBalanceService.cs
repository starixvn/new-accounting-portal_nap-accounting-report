namespace Fanex.AccountingReports.Balance.Services.Implement
{
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.AccountingReports.Balance.Repositories.Implement;
    using Fanex.AccountingReports.Balance.Resources;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;

    public class SuperBalanceService : ServiceBase
    {
        private SuperBalanceRepository _superBalanceRepository;

        public SuperBalanceService()
            : this(null)
        {
        }

        public SuperBalanceService(SuperBalanceRepository superBalanceRepository)
        {
            this._superBalanceRepository = superBalanceRepository ?? new SuperBalanceRepository();
        }

        private enum ReportRowType
        {
            Total = 1,

            GrandTotal = 2,

            NormalEvenRow = 3,

            NormalOddRow = 4
        }

        private enum BalanceType
        {
            Today = 1,

            Yesterday = 2
        }

        
        public SuperBalance GetSuperBalance(GetSuperBalanceCriteria superBalanceCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance(),
            };

            var outputParam = new Hashtable();

            if (superBalanceCriteria.Currency == 0)
            {
                superBalanceCriteria.Currency = null;
            }

            if (superBalanceCriteria.Username == null)
            {
                superBalanceCriteria.Username = string.Empty;
            }

            if (superBalanceCriteria.FirstLetter == null)
            {
                superBalanceCriteria.FirstLetter = string.Empty;
            }

            var rawData = _superBalanceRepository.GetSuperBalance(superBalanceCriteria, ref outputParam);

            if (rawData == null)
            {
                return result;
            }

            if (superBalanceCriteria.IsViewTotalOnly)
            {
                var superBalanceInfo = new SuperBalance()
                {
                    CustId = null,
                    Username = null,
                    Currency = null,
                    Balance = 0,
                    BaseBalance = Converter.ToDecimal(rawData[0]),
                    Reward = 0,
                    RewardRM = Converter.ToDecimal(rawData[1])
                };

                result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
                result.HasData = !Convert.IsDBNull(rawData[0]);
            }
            else
            {
                foreach (var row in rawData)
                {
                    var balance = Converter.ToDecimal(row.Balance);
                    var baseBalance = Converter.ToDecimal(row.BaseBalance);

                    if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                        Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                    {
                        continue;
                    }

                    var superBalanceInfo = new SuperBalance()
                    {
                        CustId = row.CustId,
                        Username = row.Username,
                        Currency = row.Currency,
                        Balance = balance,
                        BaseBalance = baseBalance,
                        HeadlineEnabled = row.HeadlineEnabled,
                        Reward = Converter.ToDecimal(row.Reward),
                        RewardRM = Converter.ToDecimal(row.RewardRM)
                    };

                    result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                    superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                    result.SuperBalanceList.Add(superBalanceInfo);
                }

                result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
            }

            return result;
        }

        
        public SuperBalance GetSuperBalanceSuperDownLine(GetSuperBalanceSuperDownlineCriteria superBalanceSuperDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            var rawData = _superBalanceRepository.GetSuperBalanceDownline(superBalanceSuperDownlineCriteria, ref outputParam);

            if (rawData == null)
            {
                return result;
            }

            foreach (var row in rawData)
            {
                var balance = Converter.ToDecimal(row.Balance);
                var baseBalance = Converter.ToDecimal(row.BaseBalance);

                if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                    Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                {
                    continue;
                }

                var superBalanceInfo = new SuperBalance()
                {
                    CustId = row.CustId,
                    Username = row.Username,
                    Currency = row.Currency,
                    Balance = balance,
                    BaseBalance = baseBalance,
                    Reward = Converter.ToDecimal(row.Reward),
                    RewardRM = Converter.ToDecimal(row.RewardRM)
                };

                result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                result.SuperBalanceList.Add(superBalanceInfo);
            }

            result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
            return result;
        }

        
        public SuperBalance GetSuperBalanceMasterDownLine(GetSuperBalanceMasterDownlineCriteria superBalanceMasterDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            var rawData = _superBalanceRepository.GetSuperBalanceMasterDownline(superBalanceMasterDownlineCriteria, ref outputParam);

            if (rawData == null)
            {
                return result;
            }

            foreach (var row in rawData)
            {
                var balance = Converter.ToDecimal(row.Balance);
                var baseBalance = Converter.ToDecimal(row.BaseBalance);

                if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                    Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                {
                    continue;
                }

                var superBalanceInfo = new SuperBalance()
                {
                    CustId = row.CustId,
                    Username = row.Username,
                    Currency = row.Currency,
                    Balance = balance,
                    BaseBalance = baseBalance,
                    Reward = Converter.ToDecimal(row.Reward),
                    RewardRM = Converter.ToDecimal(row.RewardRM)
                };

                result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                result.SuperBalanceList.Add(superBalanceInfo);
            }

            result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
            return result;
        }

        
        public SuperBalance GetSuperBalanceAgentDownLine(GetSuperBalanceAgentDownlineCriteria superBalanceAgentDownlineCriteria)
        {
            var result = new SuperBalance
            {
                SuperBalanceList = new List<SuperBalance>(),
                BaseBalanceTotal = new SuperBalance()
            };

            var outputParam = new Hashtable();

            var rawData = _superBalanceRepository.GetSuperBalanceDownline(superBalanceAgentDownlineCriteria, ref outputParam);

            if (rawData == null)
            {
                return result;
            }

            foreach (var row in rawData)
            {
                var balance = Converter.ToDecimal(row.Balance);
                var baseBalance = Converter.ToDecimal(row.BaseBalance);

                if (Math.Round(balance, 2, MidpointRounding.AwayFromZero) == 0 &&
                    Math.Round(baseBalance, 2, MidpointRounding.AwayFromZero) == 0)
                {
                    continue;
                }

                var superBalanceInfo = new SuperBalance()
                {
                    CustId = row.CustId,
                    Username = row.Username,
                    Currency = row.Currency,
                    Balance = balance,
                    BaseBalance = row.BaseBalance,
                    Reward = Converter.ToDecimal(row.Reward),
                    RewardRM = Converter.ToDecimal(row.RewardRM)
                };

                result.BaseBalanceTotal.AddToBaseBalance(superBalanceInfo);
                superBalanceInfo = SuperBalance.FormatInfo(superBalanceInfo);
                result.SuperBalanceList.Add(superBalanceInfo);
            }

            result.BaseBalanceTotal = SuperBalance.FormatInfo(result.BaseBalanceTotal);
            return result;
        }

        
        public SuperBalance ExportSuperBalanceReport(GetSuperBalanceCriteria superBalanceCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;

            var fullReport = this.GetSuperBalance(superBalanceCriteria);

            balanceType = GetBalanceType(superBalanceCriteria.BalanceType);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.SuperBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeader(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                RoleConstants.Super,
                superBalanceCriteria);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            var customerType = ReportService.GetRoleName(RoleConstants.Super);
            var reportName = customerType + balanceType + Labels.Balance;
            excelFile.ReponseStream(reportName);

            return result;
        }

        
        public SuperBalance ExportBalanceSuperDownLineReport(GetSuperBalanceSuperDownlineCriteria getSuperBalanceSuperCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;
            var fullReport = this.GetSuperBalanceSuperDownLine(getSuperBalanceSuperCriteria);

            balanceType = GetBalanceType(getSuperBalanceSuperCriteria.BalanceType);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.SuperBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeaderDownline(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                RoleConstants.Master,
                balanceType);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            var customerType = ReportService.GetRoleName(RoleConstants.Master);
            var reportName = customerType + balanceType + Labels.Balance;
            excelFile.ReponseStream(reportName);

            return result;
        }

        
        public SuperBalance ExportBalanceMasterDownLineReport(GetSuperBalanceMasterDownlineCriteria getSuperBalanceMasterCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;
            var fullReport = this.GetSuperBalanceMasterDownLine(getSuperBalanceMasterCriteria);

            balanceType = GetBalanceType(getSuperBalanceMasterCriteria.BalanceType);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.SuperBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeaderDownline(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                RoleConstants.Agent,
                balanceType);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            var customerType = ReportService.GetRoleName(RoleConstants.Agent);
            var reportName = customerType + balanceType + Labels.Balance;
            excelFile.ReponseStream(reportName);

            return result;
        }

        
        public SuperBalance ExportBalanceAgentDownLineReport(GetSuperBalanceAgentDownlineCriteria getSuperBalanceAgentCriteria)
        {
            var result = new SuperBalance();
            string balanceType = string.Empty;

            var fullReport = this.GetSuperBalanceAgentDownLine(getSuperBalanceAgentCriteria);

            balanceType = GetBalanceType(getSuperBalanceAgentCriteria.BalanceType);

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = BalanceResources.SuperBalance_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            var currentRow = startRow;

            this.ExportReportHeaderDownline(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                RoleConstants.Member,
                balanceType);

            this.ExportReportContent(
                ref workSheet,
                startRow,
                startColumn,
                ref currentRow,
                fullReport);

            var customerType = ReportService.GetRoleName(RoleConstants.Member);
            var reportName = customerType + balanceType + Labels.Balance;
            excelFile.ReponseStream(reportName);

            return result;
        }

        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();

            Dictionary<int, string> currList = ReportService.ExchangeCurrencies;

            foreach (var row in currList)
            {
                result.Add(new EntityBase<int, string>()
                {
                    Id = row.Key,
                    Name = row.Value
                });
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public List<string> GetListAlphabet()
        {
            List<string> arrLetter = new List<string>();
            arrLetter.Add("All");
            arrLetter.Add("0-9");
            for (var i = 'A'; i <= 'Z'; i++)
            {
                arrLetter.Add(((char)i).ToString());
            }

            return arrLetter;
        }

        
        public void ExportReportHeader(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int roleId,
            GetSuperBalanceCriteria superBalanceCriteria)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;
            string balanceType = string.Empty;

            balanceType = GetBalanceType(superBalanceCriteria.BalanceType);

            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            worksheet.Columns[startColumn].Width = 50 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 80 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 4].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 5].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 6].Width = 150 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;

            currentColumn = startColumn;
            var title = ReportService.GetRoleName(roleId);
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Format("{0} {1} {2}", title, balanceType, Labels.Balance);
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            if (superBalanceCriteria.IsViewTotalOnly)
            {
                return;
            }

            // First Row
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.No;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.MainWallet;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 2;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RewardWallet;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentRow++;

            currentColumn = startColumn + 3;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Balance;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BaseRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Reward;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RewardRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentRow++;
        }

        public void ExportReportHeaderDownline(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            int roleId,
            string balanceType)
        {
            CellRange currentCellRange;
            ExcelCell currentCell;

            var titleColor = ExcelExtensions.HeaderTitleColor;
            var headerColor = ExcelExtensions.HeaderTitleColor;
            worksheet.Columns[startColumn].Width = 50 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 1].Width = 120 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 2].Width = 80 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 3].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 4].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 5].Width = 150 * ExcelExtensions.FontWidth;
            worksheet.Columns[startColumn + 6].Width = 150 * ExcelExtensions.FontWidth;
            var currentColumn = startColumn;

            currentColumn = startColumn;
            var title = ReportService.GetRoleName(roleId);
            currentColumn = startColumn;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 6);
            currentCellRange.Merged = true;
            currentCellRange.Value = string.Format("{0} {1} {2}", title, balanceType, Labels.Balance);
            currentCellRange.Style.FormatTitleCell(titleColor);
            currentRow++;

            // First Row
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.No;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Currency;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.MainWallet;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentColumn += 2;

            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow, currentColumn + 1);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.RewardWallet;
            currentCellRange.Style.FormatTitleCell(headerColor);
            currentRow++;

            currentColumn = startColumn + 3;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Balance;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.BaseRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Reward;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.RewardRM;
            currentCell.Style.FormatTitleCell(headerColor);
            currentColumn++;

            currentRow++;
        }

        private void ExportReportContent(
            ref ExcelWorksheet worksheet,
            int startRow,
            int startColumn,
            ref int currentRow,
            SuperBalance report)
        {
            if (report != null &&
                report.SuperBalanceList != null &&
                report.SuperBalanceList.Count > 0)
            {
                var reportLength = report.SuperBalanceList.Count;
                var rowType = ReportRowType.NormalOddRow;
                for (int i = 0; i < reportLength; i++)
                {
                    if (i % 2 == 0)
                    {
                        rowType = ReportRowType.NormalEvenRow;
                    }
                    else
                    {
                        rowType = ReportRowType.NormalOddRow;
                    }

                    this.ExportSuperBalanceDetail(
                        ref worksheet,
                        i + 1,
                        startRow,
                        startColumn,
                        ref currentRow,
                        report.SuperBalanceList[i],
                        rowType);
                }

                this.ExportSuperBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.BaseBalanceTotal.BaseBalance, report.BaseBalanceTotal.RewardRM);
            }
            else if (report != null &&
                report.BaseBalanceTotal != null &&
                report.SuperBalanceList.Count == 0 &&
                report.HasData)
            {
                this.ExportSuperBalanceTotal(ref worksheet, startRow, startColumn, ref currentRow, report.BaseBalanceTotal.BaseBalance, report.BaseBalanceTotal.RewardRM);
            }
            else
            {
                worksheet.ExportEmptyReport(startRow, startColumn, ref currentRow, startColumn + 4);
            }
        }

        
        private void ExportSuperBalanceDetail(
           ref ExcelWorksheet worksheet,
           int index,
           int startRow,
           int startColumn,
           ref int currentRow,
           SuperBalance detail,
           ReportRowType rowType)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
            var intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.BackGroundColor,
                false,
                false,
                false);

            switch (rowType)
            {
                case ReportRowType.NormalOddRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.BackGroundColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.BackGroundColor, false, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;

                case ReportRowType.NormalEvenRow:
                    textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
                    intStyle = ExcelExtensions.GetCellNumberStyle(string.Empty, ExcelExtensions.EvenRowColor, false, false, false);
                    intStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(NumberFormatConstants.TwoDecimalDigitFormat, ExcelExtensions.EvenRowColor, false, false, false);
                    twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    break;
            }

            var currentColumn = startColumn;
            var isNormalRow = rowType == ReportRowType.NormalEvenRow ||
                rowType == ReportRowType.NormalOddRow;
            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = index;
            currentCell.Style = intStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Username;
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Currency;
            currentCell.Style = textStyle;
            currentCell.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Balance;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.BaseBalance;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.Reward;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;

            currentCell = worksheet.Cells[currentRow, currentColumn];
            currentCell.Value = detail.RewardRM;
            currentCell.Style = twoDecimalNumberStyle;
            currentColumn++;
            currentRow++;
        }

        private void ExportSuperBalanceTotal(
           ref ExcelWorksheet worksheet,
           int startRow,
           int startColumn,
           ref int currentRow,
           decimal total,
           decimal totalReward)
        {
            ExcelCell currentCell;
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, true);
            textStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            var twoDecimalNumberStyle = ExcelExtensions.GetCellNumberStyle(
                NumberFormatConstants.TwoDecimalDigitFormat,
                ExcelExtensions.GrandTotalRowColor,
                true,
                false,
                false);
            twoDecimalNumberStyle.HorizontalAlignment = HorizontalAlignmentStyle.Right;
            twoDecimalNumberStyle.Borders.SetBorders(MultipleBorders.Top, Color.FromArgb(255, 102, 0), LineStyle.Medium);
            CellRange currentCellRange;
            currentCellRange = worksheet.Cells.GetSubrangeAbsolute(currentRow, startColumn, currentRow, startColumn + 3);
            currentCellRange.Merged = true;
            currentCellRange.Value = Labels.Total;
            currentCellRange.Style = textStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            currentCell = worksheet.Cells[currentRow, startColumn + 4];
            currentCell.Value = total;
            currentCell.Style = twoDecimalNumberStyle;

            currentCell = worksheet.Cells[currentRow, startColumn + 5];
            currentCell.Value = "";
            currentCell.Style = textStyle;

            currentCell = worksheet.Cells[currentRow, startColumn + 6];
            currentCell.Value = totalReward;
            currentCell.Style = twoDecimalNumberStyle;
        }

        private string GetBalanceType(int balanceTypeVal)
        {
            string balanceType = string.Empty;

            if (balanceTypeVal == (int)BalanceType.Today)
            {
                balanceType = Labels.Today;
            }
            else
            {
                balanceType = Labels.Yesterday;
            }

            return balanceType;
        }
    }
}
