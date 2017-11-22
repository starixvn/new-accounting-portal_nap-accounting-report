namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype
{
    using Core.Configurations;
    using Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainReportService : BaseReportService, IMainReportService
    {
        private GridDataRow _soccerMinorBettypesRow;

        private GridDataRow _grandTotalRow;

        private string _soccerMinorBetTypes = string.Empty;

        private GridOptions _gridOptions { get; set; }

        private List<string> _orderBy { get; set; }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isCached">If set to <c>true</c> [is cached].</param>
        /// <returns>
        /// Grid Options
        public GridOptions GetReport(GetMainReportCriteria criteria, bool isCached = false)
        {
            if (isCached)
            {
                this._gridOptions = SessionManager.Get<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorMainSessionKey);
                if (this._gridOptions != null)
                {
                    return this._gridOptions;
                }
            }

            this._orderBy = new List<string>();
            this._orderBy.Add(Labels.Credit);
            this._orderBy.Add(Labels.Deposit);
            this._orderBy.Add(Labels._8RM);
            this._orderBy.Add(Labels.BARM);
            this._gridOptions = new GridOptions();
            this._gridOptions.GroupHeaderRows = this.GroupHeaderRows();
            this._gridOptions.CustomizeHeader = true;
            this._gridOptions.TitleType = TitleType.TwoLineBlue;
            this._gridOptions.Columns = this.HeaderRow();
            this._gridOptions.ShowDistinctRow = false;
            this._gridOptions.ShowHighlightRow = false;
            this._gridOptions.ShowTitle = true;
            this._gridOptions.ShowFooter = true;
            this._gridOptions.TitleInfoText = MainResources.DailyWinLoss2h_SoccerReport_MainReport_Title;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable param = GetParam(criteria.GetSpParams(), criteria.WinlostDate);
                db.AssignParamValues(param);
                this.Reader = db.ExecuteReader();
                while (this.Reader.Read())
                {
                    bool isRMRow = Convert.ToInt32(this.Reader["Site"]) == 0;
                    var dataRow = this.MapSPToDataRow(isRMRow);
                    this._gridOptions.BodyRows.Add(dataRow);
                    if (dataRow.Columns[0].Value.ToString() == Labels.Credit || string.IsNullOrEmpty(this._soccerMinorBetTypes))
                    {
                        this._soccerMinorBetTypes = Convert.ToString(this.Reader["SportName"]);
                    }
                }
            }

            this.RefineData();
            SessionManager.Set<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorMainSessionKey, this._gridOptions);
            return this._gridOptions;
        }

        public Hashtable GetParam(Hashtable param, DateTime winlossDate)
        {
            if (ReportService.IsRequestDataWH(winlossDate))
            {
                param.Add("linetotal", 0);
                param.Add("IncludeBetType", AppSettings.Main2hSoccerMinorBettypes);
            }

            return param;
        }

        /// <summary>
        /// Maps the SP to data row.
        /// </summary>
        /// <param name="isRMRow">If set to <c>true</c> [is RM row].</param>
        /// <returns>Grid Data Row.</returns>
        private GridDataRow MapSPToDataRow(bool isRMRow)
        {
            var dataRow = new GridDataRow();
            double realLineTotal = Convert.ToDouble(this.Reader["RealLineTotal"]);
            double virtualLineTotal = Convert.ToDouble(this.Reader["VirtualLineTotal"]);
            double realTurnOver = Convert.ToDouble(this.Reader["RealTurnOver"]);
            double virtualTurnOver = Convert.ToDouble(this.Reader["VirtualTurnOver"]);
            double realMargin = realTurnOver == 0 ? 0 : realLineTotal / realTurnOver;
            double virtualMargin = virtualTurnOver == 0 ? 0 : virtualLineTotal / virtualTurnOver;
            double variance = realLineTotal - virtualLineTotal;
            string cellClass = isRMRow ? "subTotal" : string.Empty;
            int ibc = Convert.ToInt32(this.Reader["IBC"]);
            int site = Convert.ToInt32(this.Reader["Site"]);
            int currentColIndex = 0;
            string sportNameColumnValue = this.GetSportNameColumnText(ibc, site);
            var align = HAlignment.Right;
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.SportType, sportNameColumnValue, cellClass, align, true));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.WinLoss, Converter.ToDecimal(this.Reader["WinLost"]), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Position, Converter.ToDecimal(this.Reader["Position"]), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Commission, Converter.ToDecimal(this.Reader["Commission"]), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.LineTotal + Labels.Real, Converter.ToDecimal(realLineTotal), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.LineTotal + Labels.Virtual, Converter.ToDecimal(virtualLineTotal), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Variance, Converter.ToDecimal(variance), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Turnover + Labels.Real, Converter.ToDecimal(realTurnOver), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Turnover + Labels.Virtual, Converter.ToDecimal(virtualTurnOver), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Margin + Labels.Real, Converter.ToDecimal(realMargin), cellClass, align, isRMRow));
            dataRow.Columns.Add(this.CreateColumns(currentColIndex++, Labels.Margin + Labels.Virtual, Converter.ToDecimal(virtualMargin), cellClass, align, isRMRow));
            if (ibc == 0)
            {
                dataRow.CssClass = "data-group-0";
            }

            return dataRow;
        }

        private Columns CreateColumns(int colIndex, string name, object value, string cellClass, HAlignment cellAlign, bool isBold = false)
        {
            return new Columns(colIndex) { Name = name, Value = value, TDClass = cellClass, CellAlignment = cellAlign, Bold = isBold };
        }

        /// <summary>
        /// Gets the sport name column text.
        /// </summary>
        /// <param name="ibc">The IBC value.</param>
        /// <param name="site">The site value.</param>
        /// <returns>Sport Name.</returns>
        private string GetSportNameColumnText(int ibc, int site)
        {
            if (ibc == 0)
            {
                return site == 1 ? Labels.Credit : Labels.Deposit;
            }
            else if (ibc == 1)
            {
                return Labels._8RM;
            }
            else if (ibc == 2)
            {
                return Labels.BARM;
            }

            return string.Empty;
        }

        /// <summary>
        /// Refines the data.
        /// </summary>
        private void RefineData()
        {
            var bodyRowsCount = this._gridOptions.BodyRows.Count;
            if (bodyRowsCount == 0)
            {
                return;
            }

            this._gridOptions.BodyRows = this._gridOptions.BodyRows.OrderBy(m => this.GetSortingOrder(m.Columns[0].Value.ToString())).ToList();

            // Calculate Sporttype TotalRow
            this._soccerMinorBettypesRow = this._gridOptions.BodyRows[0].Clone();
            this._soccerMinorBettypesRow.IsEven = true;
            this._soccerMinorBettypesRow.Columns[0].CellAlignment = HAlignment.Left;
            if (this._gridOptions.BodyRows.Count > 1)
            {
                this._soccerMinorBettypesRow.AddValue(this._gridOptions.BodyRows[1]);
            }

            this._soccerMinorBettypesRow.Columns[0].Value = this._soccerMinorBetTypes;
            this._soccerMinorBettypesRow.CssClass = "freegrid-even";

            this.RecalculateVarianceAndMarginValue(this._soccerMinorBettypesRow);
            this._grandTotalRow = this._gridOptions.BodyRows[0].Clone();
            this._grandTotalRow.CssClass = string.Empty;
            this._grandTotalRow.Columns[0].Value = Labels.Grand_Total;
            this._grandTotalRow.Columns[0].CellAlignment = HAlignment.Right;
            for (int i = 1; i < bodyRowsCount; i++)
            {
                this._grandTotalRow.AddValue(this._gridOptions.BodyRows[i]);
            }

            this.RecalculateVarianceAndMarginValue(this._grandTotalRow);
            this._gridOptions.BodyRows.Insert(0, this._soccerMinorBettypesRow);
            this._gridOptions.FooterRows.Add(this._grandTotalRow);

            GridDataRow grandTotalInfoRow = new GridDataRow();
            grandTotalInfoRow.Columns.Add(new Columns(0)
            {
                Name = Labels.SportType,
                Value = MainResources.DailyWinLoss2hSoccer_ReportNote,
                CustomCellType = ExcelCellType.FooterInformation,
                MergeColNumber = 11,
                CellAlignment = HAlignment.Left,
                TDClass = "grand-total-note"
            });
            this._gridOptions.FooterRows.Add(grandTotalInfoRow);
        }

        /// <summary>
        /// Gets the sorting order.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Order number.</returns>
        private int GetSortingOrder(string value)
        {
            for (int i = 0; i < this._orderBy.Count; i++)
            {
                if (this._orderBy[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
