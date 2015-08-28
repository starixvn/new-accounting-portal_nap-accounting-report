// -----------------------------------------------------------------------
// <copyright file="KenoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Implement.PlaytechCasinoWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria.KenoWinLoss;
    using Fanex.AccountingReports.Licensee.Enum;
    using Fanex.AccountingReports.Licensee.Models.KenoWinLoss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.CellStyles;
    using Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.Color;
    using Fanex.Common;
    using Fanex.Data;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Play tech casino win loss service.
    /// </summary>
    public class KenoWinLossService : ServiceBase, IKenoWinLossService
    {
        private HSSFWorkbook _workbook;
        private ISheet _worksheet;
        private int _startRow = 0;
        private int _startCol = 0;
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _countryMarket;
        private KenoWinLoss _reportData;
        private KenoWinLossCellStyles _excelStyles;
        private CustomColor _customColor;
        private List<string> _header;
        private List<CellRangeAddress> _listMergeRegion;

        /// <summary>
        /// Gets the list of licensees.
        /// </summary>
        /// <returns>Licensees list.</returns>
        public List<SelectListItem> GetListLicensees()
        {
            var result = new List<SelectListItem>();
            var rawData = GetReportData<dynamic>(new GetKenoLicenseeCriteria());
            foreach (var row in rawData)
            {
                result.Add(new SelectListItem()
                {
                    Text = row.LicenseeName,
                    Value = row.LicenseeID.ToString(),
                    Selected = false
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the list country market.
        /// </summary>
        /// <returns>Country market list.</returns>
        public List<SelectListItem> GetListCountryMarket()
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0",
                Selected = true,
            });

            var rawData = GetReportData<dynamic>(new GetKenoCountryMarketCriteria());
            foreach (var row in rawData)
            {
                result.Add(new SelectListItem()
                {
                    Text = row.BettypeName,
                    Value = row.BettypeId.ToString(),
                    Selected = false
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the list country market.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// Country market list.
        /// </returns>
        public List<SelectListItem> GetListCurrency(GetKenoCurrencyByLicenseeCriteria criteria)
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0",
                Selected = true,
            });

            var rawData = GetReportData<dynamic>(criteria);
            foreach (var row in rawData)
            {
                result.Add(new SelectListItem()
                {
                    Text = row.CurrencyCode,
                    Value = row.CurrencyCode,
                    Selected = false
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the keno win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Keno Win Loss collection.</returns>
        public KenoWinLoss GetKenoWinLossReport(GetKenoWinLossCriteria criteria)
        {
            KenoWinLoss result = new KenoWinLoss();
            var rawData = GetReportData<dynamic>(criteria);
            KenoWinLoss totalRow = new KenoWinLoss();
            foreach (var row in rawData)
            {
                var reportRow = new KenoWinLoss()
                {
                    Date = Converter.ToDateTime(row.rptDate),
                    Currency = row.Currency,
                    TurnOver = Converter.ToDecimal(row.TurnOver),
                    TotalTransaction = Converter.ToDecimal(row.TotalTransaction),
                    CustomerWinLoss = Converter.ToDecimal(row.GrossWin)
                };

                result.ReportRows.Add(reportRow);
                totalRow.TurnOver += reportRow.TurnOver;
                totalRow.TotalTransaction += reportRow.TotalTransaction;
                totalRow.CustomerWinLoss += reportRow.CustomerWinLoss;
            }

            totalRow.Currency = "Total";
            result.Total = totalRow;
            SessionManager.Set<KenoWinLoss>(LicenseeConstants.KenoWinLossReportSessionKey, result);
            return result;
        }

        /// <summary>
        /// Gets the keno win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Keno Win Loss collection.</returns>
        public GridOptions GetKenoWinLossReport2(GetKenoWinLossCriteria criteria)
        {
            int startMergeRow = 0;
            int endMergeRow = 0;
            bool oddEven = false;
            GridDataRow currentRow = null;
            GridDataRow previousRow = null;
            GridOptions result = new GridOptions();
            result.ShowTitle = true;
            result.ShowFooter = true;
            result.Columns = this.GetHeaderColumns();
            result.TitleInfoText = string.Format(
                Labels.KenoWinlossGridTitle,
                criteria.LicenseeFileNameString.Replace(",", "_"),
                criteria.FromDate.ToString(FormatConstants.MMMDDYYYYString),
                criteria.ToDate.ToString(FormatConstants.MMMDDYYYYString),
                criteria.Currency.Equals("0") ? "All Currencies" : (criteria.Currency + " Currency"),
                this.GetListCountryMarket().Where(m => m.Value.Equals(criteria.CountryMarket)).FirstOrDefault().Text);
            result.NoDataText = Labels.NoInfo;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                ReadBodyDataPart(ref startMergeRow, ref endMergeRow, ref oddEven, ref currentRow, ref previousRow, result, reader);
                if (endMergeRow > startMergeRow)
                {
                    result.BodyRows[startMergeRow].Columns[0].MergeRowNumber = endMergeRow - startMergeRow;
                }

                reader.NextResult();
                ReadSubTotalDataPart(result, reader);
                reader.NextResult();
                ReadGrandTotalDataPart(result, reader);
            }

            result.ShowFooter = result.BodyRows.Count != 0;
            SessionManager.Set<GridOptions>(LicenseeConstants.KenoWinLossReportSessionKey, result);
            return result;
        }

        /// <summary>
        /// Exports the keno win loss report.
        /// </summary>
        /// <param name="criteria">The Keno Win Loss criteria.</param>
        public void ExportKenoWinLossReport(GetKenoWinLossCriteria criteria)
        {
            try
            {
                this._listMergeRegion = new List<CellRangeAddress>();
                var listCountryMarket = this.GetListCountryMarket();
                var gridOptions = SessionManager.Get<GridOptions>(LicenseeConstants.KenoWinLossReportSessionKey);
                this.ConvertBackToKenoWinloss(gridOptions);
                if (this._reportData == null)
                {
                    this._reportData = this.GetKenoWinLossReport(criteria);
                }

                this._fromDate = criteria.FromDate;
                this._toDate = criteria.ToDate;
                this._countryMarket = listCountryMarket.Where(m => m.Value.Equals(criteria.CountryMarket)).FirstOrDefault().Text;
                this.RenderFile(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void ReadGrandTotalDataPart(GridOptions result, IDataReader reader)
        {
            while (reader.Read())
            {
                result.FooterRows = this.MapDataReaderToGridGrandTotalDataRow(reader);
            }
        }

        /// <summary>
        /// Reads the sub total data part.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="reader">The data reader.</param>
        private void ReadSubTotalDataPart(GridOptions result, IDataReader reader)
        {
            int i = 0;
            int currentIndex = result.BodyRows.Count;
            while (reader.Read())
            {
                var row = this.MapDataReaderToGridSubTotalDataRow(reader);
                if (i != 0)
                {
                    row.Columns.RemoveAt(0);
                }

                result.BodyRows.Add(row);
                ++i;
            }

            if (result.BodyRows.Count != 0)
            {
                result.BodyRows[currentIndex].Columns[0].MergeRowNumber = i;
            }
        }

        /// <summary>
        /// Reads the body data part.
        /// </summary>
        /// <param name="startMergeRow">The start merge row.</param>
        /// <param name="endMergeRow">The end merge row.</param>
        /// <param name="oddEven">If set to <c>true</c> [odd even].</param>
        /// <param name="currentRow">The current row.</param>
        /// <param name="previousRow">The previous row.</param>
        /// <param name="result">The result.</param>
        /// <param name="reader">The reader.</param>
        private void ReadBodyDataPart(
            ref int startMergeRow, 
            ref int endMergeRow, 
            ref bool oddEven, 
            ref GridDataRow currentRow, 
            ref GridDataRow previousRow, 
            GridOptions result, 
            IDataReader reader)
        {
            while (reader.Read())
            {
                currentRow = this.MapDataReaderToGridDataRow(reader);
                if (CompareRow(currentRow, previousRow) == KenoWinlossChangeType.DateChange)
                {
                    result.BodyRows[startMergeRow].Columns[0].MergeRowNumber = endMergeRow - startMergeRow;
                    startMergeRow = endMergeRow;
                    previousRow = currentRow;
                    currentRow.CssClass = oddEven ? "freegrid-even" : string.Empty;
                    oddEven = !oddEven;
                }
                else
                {
                    if (previousRow != null)
                    {
                        currentRow.Columns.RemoveAt(0);
                        currentRow.CssClass = !oddEven ? "freegrid-even" : string.Empty;
                    }
                    else
                    {
                        previousRow = currentRow;
                        currentRow.CssClass = oddEven ? "freegrid-even" : string.Empty;
                        oddEven = !oddEven;
                    }
                }

                result.BodyRows.Add(currentRow);
                ++endMergeRow;
            }
        }

        /// <summary>
        /// Gets the header columns.
        /// </summary>
        /// <returns>Header columns.</returns>
        private List<HeaderColumns> GetHeaderColumns()
        {
            List<HeaderColumns> header = new List<HeaderColumns>();
            header.Add(this.GetDateHeader());
            header.Add(this.GetCurrencyHeader());
            header.Add(this.GetTurnoverHeader());
            header.Add(this.GetTotalTransactionsHeader());
            header.Add(this.GetCustomerWLHeader());
            return header;
        }

        /// <summary>
        /// Gets the date header.
        /// </summary>
        /// <returns>Date header.</returns>
        private HeaderColumns GetDateHeader()
        {
            return new HeaderColumns()
            {
                HeaderText = Labels.Date,
                Name = "rptDate",
                Width = 200,
                HeaderAlign = FormatConstants.AlignCenter,
                CellAlign = FormatConstants.AlignCenter,
                CellCssClass = "center",
                FormatDateTime = FormatConstants.MMDDYYYYWithSlashString
            };
        }

        /// <summary>
        /// Gets the currency header.
        /// </summary>
        /// <returns>Currency header.</returns>
        private HeaderColumns GetCurrencyHeader()
        {
            return new HeaderColumns()
            {
                HeaderText = Labels.Currency,
                Name = Labels.Currency,
                Width = 200,
                HeaderAlign = FormatConstants.AlignCenter,
                CellAlign = FormatConstants.AlignCenter
            };
        }

        /// <summary>
        /// Gets the turnover header.
        /// </summary>
        /// <returns>Turnover Header.</returns>
        private HeaderColumns GetTurnoverHeader()
        {
            return new HeaderColumns()
            {
                HeaderText = Labels.Turnover,
                Name = "TurnOver",
                Width = 200,
                HeaderAlign = FormatConstants.AlignCenter,
                CellAlign = FormatConstants.AlignRight,
                FormatNumber = FormatConstants.TwoDecimalNumber
            };
        }

        /// <summary>
        /// Gets the total transactions header.
        /// </summary>
        /// <returns>Total transactions header.</returns>
        private HeaderColumns GetTotalTransactionsHeader()
        {
            return new HeaderColumns()
            {
                HeaderText = Labels.TotalTransactions,
                Name = "TotalTransaction",
                Width = 200,
                HeaderAlign = FormatConstants.AlignCenter,
                CellAlign = FormatConstants.AlignRight,
                FormatNumber = FormatConstants.IntNumber
            };
        }

        /// <summary>
        /// Gets the customer Win loss header.
        /// </summary>
        /// <returns>Win loss Header column.</returns>
        private HeaderColumns GetCustomerWLHeader()
        {
            return new HeaderColumns()
            {
                HeaderText = Labels.CustomerWL,
                Name = "CustomerWL",
                Width = 200,
                HeaderAlign = FormatConstants.AlignCenter,
                CellAlign = FormatConstants.AlignRight,
                FormatNumber = FormatConstants.TwoDecimalNumber
            };
        }

        /// <summary>
        /// Maps the data reader to grid data row.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Grid Data Row.</returns>
        private GridDataRow MapDataReaderToGridDataRow(IDataReader reader)
        {
            int fieldCount = reader.FieldCount;
            GridDataRow row = new GridDataRow();
            for (int i = 0; i < fieldCount; i++)
            {
                row.Columns.Add(new Columns()
                {
                    Name = reader.GetName(i),
                    Value = reader[i]
                });
            }

            return row;
        }

        /// <summary>
        /// Compares the row.
        /// </summary>
        /// <param name="source">The source row.</param>
        /// <param name="destination">The destination row.</param>
        /// <returns>Change Type.</returns>
        private KenoWinlossChangeType CompareRow(GridDataRow source, GridDataRow destination)
        {
            if (source == null || destination == null)
            {
                return KenoWinlossChangeType.NothingChange;
            }

            try
            {
                if (((DateTime)source.Columns[0].Value).CompareTo((DateTime)destination.Columns[0].Value) != 0)
                {
                    return KenoWinlossChangeType.DateChange;
                }
            }
            catch
            {
            }

            return KenoWinlossChangeType.NothingChange;
        }

        /// <summary>
        /// Maps the data reader to grid sub total data row.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Grid Data Row.</returns>
        private GridDataRow MapDataReaderToGridSubTotalDataRow(IDataReader reader)
        {
            GridDataRow row = new GridDataRow();
            row.Columns.Add(new Columns()
            {
                Name = "rptDate",
                Value = "Total",
                TdClass = "subTotal"
            });
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                row.Columns.Add(new Columns()
                {
                    Name = reader.GetName(i),
                    Value = reader[i],
                    TdClass = "subTotal"
                });
            }

            return row;
        }

        /// <summary>
        /// Maps the data reader to grid grand total data row.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>List Grid Data Row.</returns>
        private List<GridDataRow> MapDataReaderToGridGrandTotalDataRow(IDataReader reader)
        {
            List<GridDataRow> result = new List<GridDataRow>();
            GridDataRow totalRow = new GridDataRow();
            totalRow.Columns.Add(new Columns() { Name = "rptDate", Value = "Total", MergeRowNumber = 3 });
            totalRow.Columns.Add(new Columns() { Name = "Currency", Value = "EUR" });
            totalRow.Columns.Add(new Columns() { Name = "TurnOver", Value = reader["ToTalTurnOverEUR"] });
            totalRow.Columns.Add(new Columns() { Name = "TotalTransaction", Value = reader["TotalTransaction"], MergeRowNumber = 3 });
            totalRow.Columns.Add(new Columns() { Name = "CustomerWL", Value = reader["CustomerWLEUR"] });
            result.Add(totalRow);
            totalRow = new GridDataRow();
            totalRow.Columns.Add(new Columns() { Name = "Currency", Value = "MYR" });
            totalRow.Columns.Add(new Columns() { Name = "TurnOver", Value = reader["ToTalTurnOverMYR"] });
            totalRow.Columns.Add(new Columns() { Name = "CustomerWL", Value = reader["CustomerWLMYR"] });
            result.Add(totalRow);
            totalRow = new GridDataRow();
            totalRow.Columns.Add(new Columns() { Name = "Currency", Value = Labels.GBP });
            totalRow.Columns.Add(new Columns() { Name = "TurnOver", Value = reader["ToTalTurnOverGBP"] });
            totalRow.Columns.Add(new Columns() { Name = "CustomerWL", Value = reader["CustomerWLGBP"] });
            result.Add(totalRow);
            return result;
        }

        /// <summary>
        /// Converts the back to keno win loss.
        /// </summary>
        /// <param name="gridOptions">The grid options.</param>
        private void ConvertBackToKenoWinloss(GridOptions gridOptions)
        {
            this._reportData = new KenoWinLoss();
            this._reportData.SubTotal = new KenoWinLoss();
            this._reportData.Total = new KenoWinLoss();
            string cachedDate = string.Empty;
            int startBodyDataRow = 3;
            int startMergeRow = startBodyDataRow;
            int endMergeRow = startBodyDataRow;
            bool oddEven = true;
            for (int i = 0; i < gridOptions.BodyRows.Count; i++)
            {
                ConvertDataRow(gridOptions, ref cachedDate, startBodyDataRow, ref startMergeRow, ref endMergeRow, ref oddEven, i);
            }

            this._reportData.Total.ReportRows.Add(new KenoWinLoss()
            {
                Date = Labels.Total,
                Currency = gridOptions.FooterRows[0].Columns[1].Value.ToString(),
                TurnOver = decimal.Parse(gridOptions.FooterRows[0].Columns[2].Value.ToString()),
                TotalTransaction = decimal.Parse(gridOptions.FooterRows[0].Columns[3].Value.ToString()),
                CustomerWinLoss = decimal.Parse(gridOptions.FooterRows[0].Columns[4].Value.ToString())
            });

            this._reportData.Total.ReportRows.Add(new KenoWinLoss()
            {
                Date = Labels.Total,
                Currency = gridOptions.FooterRows[1].Columns[0].Value.ToString(),
                TurnOver = decimal.Parse(gridOptions.FooterRows[1].Columns[1].Value.ToString()),
                TotalTransaction = this._reportData.Total.ReportRows[0].TotalTransaction,
                CustomerWinLoss = decimal.Parse(gridOptions.FooterRows[1].Columns[2].Value.ToString())
            });

            this._reportData.Total.ReportRows.Add(new KenoWinLoss()
            {
                Date = Labels.Total,
                Currency = gridOptions.FooterRows[2].Columns[0].Value.ToString(),
                TurnOver = decimal.Parse(gridOptions.FooterRows[2].Columns[1].Value.ToString()),
                TotalTransaction = this._reportData.Total.ReportRows[0].TotalTransaction,
                CustomerWinLoss = decimal.Parse(gridOptions.FooterRows[2].Columns[2].Value.ToString())
            });
        }

        /// <summary>
        /// Converts the data row.
        /// </summary>
        /// <param name="gridOptions">The grid options.</param>
        /// <param name="cachedDate">The cached date.</param>
        /// <param name="startBodyDataRow">The start body data row.</param>
        /// <param name="startMergeRow">The start merge row.</param>
        /// <param name="endMergeRow">The end merge row.</param>
        /// <param name="oddEven">If set to <c>true</c> [odd even].</param>
        /// <param name="index">The row index.</param>
        private void ConvertDataRow(
            GridOptions gridOptions, 
            ref string cachedDate, 
            int startBodyDataRow, 
            ref int startMergeRow, 
            ref int endMergeRow, 
            ref bool oddEven, 
            int index)
        {
            var currentRow = gridOptions.BodyRows[index];
            int isMergeRow = currentRow.Columns.Count == 5 ? 0 : 1;
            if (isMergeRow == 0)
            {
                if (currentRow.Columns[0].Value.ToString().Equals(Labels.Total))
                {
                    cachedDate = currentRow.Columns[0].Value.ToString();
                }
                else
                {
                    cachedDate = DateTime.Parse(currentRow.Columns[0].Value.ToString()).ToString("MM/dd/yyyy");
                }

                if (index != 0)
                {
                    endMergeRow = index + startBodyDataRow - 1;
                    this._listMergeRegion.Add(new CellRangeAddress(startMergeRow, endMergeRow, 0, 0));
                    startMergeRow = index + startBodyDataRow;
                    oddEven = !oddEven;
                }
            }

            if (currentRow.Columns[0].Value.ToString().Equals(Labels.Total) || cachedDate.Equals(Labels.Total))
            {
                this._reportData.SubTotal.ReportRows.Add(new KenoWinLoss()
                {
                    Date = Labels.Total,
                    Currency = currentRow.Columns[1 - isMergeRow].Value.ToString(),
                    TurnOver = decimal.Parse(currentRow.Columns[2 - isMergeRow].Value.ToString()),
                    TotalTransaction = decimal.Parse(currentRow.Columns[3 - isMergeRow].Value.ToString()),
                    CustomerWinLoss = decimal.Parse(currentRow.Columns[4 - isMergeRow].Value.ToString())
                });
            }
            else
            {
                this._reportData.ReportRows.Add(new KenoWinLoss()
                {
                    Date = cachedDate,
                    Currency = currentRow.Columns[1 - isMergeRow].Value.ToString(),
                    TurnOver = decimal.Parse(currentRow.Columns[2 - isMergeRow].Value.ToString()),
                    TotalTransaction = decimal.Parse(currentRow.Columns[3 - isMergeRow].Value.ToString()),
                    CustomerWinLoss = decimal.Parse(currentRow.Columns[4 - isMergeRow].Value.ToString()),
                    OddEven = oddEven
                });
            }
        }

        private void GetReportHeader()
        {
            this._header = new List<string>();
            this._header.Add(Labels.Date);
            this._header.Add(Labels.Currency);
            this._header.Add(Labels.Turnover);
            this._header.Add(Labels.TotalTransactions);
            this._header.Add(Labels.CustomerWL);
        }

        /// <summary>
        /// Renders the file.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void RenderFile(GetKenoWinLossCriteria criteria)
        {
            this._workbook = new HSSFWorkbook();
            this._worksheet = this._workbook.CreateSheet(LicenseeResources.KenoWinLoss_SheetName);
            this._customColor = new CustomColor(this._workbook);
            this._excelStyles = new KenoWinLossCellStyles(this._workbook, this._customColor);
            this.RenderReportTitle(criteria);
            this.RenderReportBody();

            for (int i = 0; i < this._listMergeRegion.Count; i++)
            {
                this._worksheet.AddMergedRegion(this._listMergeRegion[i]);
            }

            string fileName = string.Format(
                LicenseeResources.KenoWinLoss_ExcelFileName,
                criteria.LicenseeFileNameString.Replace(",", "_").Replace(" ", "_"),
                criteria.FromDate.ToString("M_d_yyyy"),
                criteria.ToDate.ToString("M_d_yyyy"),
                criteria.Currency.Equals("0") ? "All_Currencies" : (criteria.Currency + "_Currency"),
                this.GetListCountryMarket().Where(m => m.Value.Equals(criteria.CountryMarket)).FirstOrDefault().Text);
            NpoiExcelExtensions.ResponseStreamExcel(this._workbook, fileName);
        }

        /// <summary>
        /// Renders the report title.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void RenderReportTitle(GetKenoWinLossCriteria criteria)
        {
            var row = this._worksheet.CreateRow(this._startRow++);
            var cell = row.CreateCell(this._startCol);
            cell.CellStyle = this._excelStyles.ReportTitle;
            cell.SetCellValue(
                string.Format(
                    LicenseeResources.KenoWinLoss_ReportTitle,
                    criteria.LicenseeFileNameString.Replace(",", "_"),
                    criteria.FromDate.ToString(FormatConstants.MMMDDYYYYString),
                    criteria.ToDate.ToString(FormatConstants.MMMDDYYYYString),
                    criteria.Currency.Equals("0") ? "All Currencies" : (criteria.Currency + " Currency"),
                    this.GetListCountryMarket().Where(m => m.Value.Equals(criteria.CountryMarket)).FirstOrDefault().Text));
            for (int i = 1; i < 5; i++)
            {
                cell = row.CreateCell(this._startCol + i);
                cell.CellStyle = this._excelStyles.ReportTitle;
            }

            this._startRow++;
        }

        private void RenderReportBody()
        {
            this.RenderReportBodyHeaderRow();
            this.RenderReportBodyDataRows();
            this.RenderReportBodySubTotalRows();
            this.RenderReportBodyGrandTotalRows();
        }

        /// <summary>
        /// Renders the report body header row.
        /// </summary>
        private void RenderReportBodyHeaderRow()
        {
            this.GetReportHeader();
            var currentColumn = this._startCol;
            var row = this._worksheet.CreateRow(this._startRow++);
            var fieldCount = this._header.Count;
            for (int i = 0; i < fieldCount; i++)
            {
                var currentCell = row.CreateCell(i);
                currentCell.CellStyle = this._excelStyles.ReportBodyHeader;
                currentCell.SetCellValue(this._header[i]);
            }
        }

        private void RenderReportBodyDataRows()
        {
            var rowCount = this._reportData.ReportRows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                this.RenderReportBodyDataRow(i);
            }
        }

        /// <summary>
        /// Renders the report body data row.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        private void RenderReportBodyDataRow(int rowIndex)
        {
            var row = this._worksheet.CreateRow(this._startRow++);
            var currentColumn = this._startCol;
            bool oddEven = this._startRow % 2 == 0;
            this.RenderDataCell(
                row.CreateCell(currentColumn++), 
                this._reportData.ReportRows[rowIndex].OddEven, 
                this._reportData.ReportRows[rowIndex].Date);
            this.RenderDataCell(
                row.CreateCell(currentColumn++), 
                this._reportData.ReportRows[rowIndex].OddEven, 
                this._reportData.ReportRows[rowIndex].Currency);
            this.RenderDataCell(
                row.CreateCell(currentColumn++), 
                this._reportData.ReportRows[rowIndex].OddEven, 
                this._reportData.ReportRows[rowIndex].TurnOver);
            this.RenderDataCell(
                row.CreateCell(currentColumn++), 
                this._reportData.ReportRows[rowIndex].OddEven, 
                this._reportData.ReportRows[rowIndex].TotalTransaction, 
                true);
            this.RenderDataCell(
                row.CreateCell(currentColumn++), 
                this._reportData.ReportRows[rowIndex].OddEven, 
                this._reportData.ReportRows[rowIndex].CustomerWinLoss);
        }

        /// <summary>
        /// Renders the data cell.
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="oddOrEven">If set to <c>true</c> [odd or even].</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="isRounded">If set to <c>true</c> [is rounded].</param>
        private void RenderDataCell(ICell cell, bool oddOrEven, object cellValue, bool isRounded = false)
        {
            if (cellValue is string)
            {
                cell.CellStyle = oddOrEven ? this._excelStyles.ReportBodyData.OddText : this._excelStyles.ReportBodyData.EvenText;
            }
            else if (isRounded)
            {
                cell.CellStyle = oddOrEven ? this._excelStyles.ReportBodyData.OddInteger : this._excelStyles.ReportBodyData.EvenInteger;
            }
            else
            {
                cell.CellStyle = oddOrEven ? this._excelStyles.ReportBodyData.OddDecimal : this._excelStyles.ReportBodyData.EvenDecimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
        }

        /// <summary>
        /// Renders the report body sub total rows.
        /// </summary>
        private void RenderReportBodySubTotalRows()
        {
            var rowCount = this._reportData.SubTotal.ReportRows.Count;
            int startMergeRegion = this._startRow;
            int endMergeRegion = this._startRow;
            for (int i = 0; i < rowCount; i++)
            {
                this.RenderReportBodySubTotalRow(i);
            }

            endMergeRegion = this._startRow - 1;
            this._listMergeRegion.Add(new CellRangeAddress(startMergeRegion, endMergeRegion, 0, 0));
        }

        private void RenderReportBodySubTotalRow(int rowIndex)
        {
            var row = this._worksheet.CreateRow(this._startRow++);
            var currentColumn = this._startCol;
            this.RenderSubTotalCell(row.CreateCell(currentColumn++), this._reportData.SubTotal.ReportRows[rowIndex].Date);
            this.RenderSubTotalCell(row.CreateCell(currentColumn++), this._reportData.SubTotal.ReportRows[rowIndex].Currency);
            this.RenderSubTotalCell(row.CreateCell(currentColumn++), this._reportData.SubTotal.ReportRows[rowIndex].TurnOver);
            this.RenderSubTotalCell(row.CreateCell(currentColumn++), this._reportData.SubTotal.ReportRows[rowIndex].TotalTransaction, true);
            this.RenderSubTotalCell(row.CreateCell(currentColumn++), this._reportData.SubTotal.ReportRows[rowIndex].CustomerWinLoss);
        }

        /// <summary>
        /// Renders the report body grand total rows.
        /// </summary>
        private void RenderReportBodyGrandTotalRows()
        {
            var rowCount = this._reportData.Total.ReportRows.Count;
            int startMergeRegion = this._startRow;
            for (int i = 0; i < rowCount; i++)
            {
                this.RenderReportBodyGrandTotalRow(i);
            }

            int endMergeRegion = this._startRow - 1;
            this._listMergeRegion.Add(new CellRangeAddress(startMergeRegion, endMergeRegion, 0, 0));
            this._listMergeRegion.Add(new CellRangeAddress(startMergeRegion, endMergeRegion, 3, 3));
        }

        private void RenderReportBodyGrandTotalRow(int rowIndex)
        {
            var row = this._worksheet.CreateRow(this._startRow++);
            var currentColumn = this._startCol;
            this.RenderTotalCell(row.CreateCell(currentColumn++), this._reportData.Total.ReportRows[rowIndex].Date, rowIndex == 0);
            this.RenderTotalCell(row.CreateCell(currentColumn++), this._reportData.Total.ReportRows[rowIndex].Currency, rowIndex == 0);
            this.RenderTotalCell(row.CreateCell(currentColumn++), this._reportData.Total.ReportRows[rowIndex].TurnOver, rowIndex == 0);
            this.RenderTotalCell(row.CreateCell(currentColumn++), this._reportData.Total.ReportRows[rowIndex].TotalTransaction, rowIndex == 0, true);
            this.RenderTotalCell(row.CreateCell(currentColumn++), this._reportData.Total.ReportRows[rowIndex].CustomerWinLoss, rowIndex == 0);
        }

        /// <summary>
        /// Render the total cell.       
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="isUpper">If set to <c>true</c> [is upper].</param>
        /// <param name="isRounded">If set to <c>true</c> [is rounded].</param>        
        private void RenderTotalCell(ICell cell, object cellValue, bool isUpper = false, bool isRounded = false)
        {
            if (cellValue is string)
            {
                cell.CellStyle = isUpper ? this._excelStyles.ReportBodyFooter.TextUpper : this._excelStyles.ReportBodyFooter.Text;
            }
            else if (isRounded)
            {
                cell.CellStyle = isUpper ? this._excelStyles.ReportBodyFooter.IntegerUpper : this._excelStyles.ReportBodyFooter.Integer;
            }
            else
            {
                cell.CellStyle = isUpper ? this._excelStyles.ReportBodyFooter.DecimalUpper : this._excelStyles.ReportBodyFooter.Decimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
            this._worksheet.SetColumnWidth(cell.ColumnIndex, 140 * ExcelExtensions.FontWidth);
        }

        /// <summary>
        /// Renders the total cell.
        /// </summary>
        /// <param name="cell">The current cell.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <param name="isRounded">If set to <c>true</c> [is rounded].</param>
        private void RenderSubTotalCell(ICell cell, object cellValue, bool isRounded = false)
        {
            if (cellValue is string)
            {
                cell.CellStyle = this._excelStyles.ReportBodySubTotal.Text;
            }
            else if (isRounded)
            {
                cell.CellStyle = this._excelStyles.ReportBodySubTotal.Integer;
            }
            else
            {
                cell.CellStyle = this._excelStyles.ReportBodySubTotal.Decimal;
            }

            NpoiExcelExtensions.SetCellValue(cell, cellValue);
        }
    }
}