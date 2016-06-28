// -----------------------------------------------------------------------
// <copyright file="SiteReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SiteReportService : BaseReportService
    {
        private GridDataRow _grandTotalRow;

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isCached">If set to <c>true</c> [is cached].</param>
        /// <returns>
        /// Grid Options.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public GridOptions GetReport(GetWinLossBySite_BetTypeCriteria criteria, bool isCached = false)
        {
            if (isCached)
            {
                this.GridOptions = SessionManager.Get<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorSiteSessionKey);
                if (this.GridOptions != null)
                {
                    return this.GridOptions;
                }
            }

            this.GridOptions = new GridOptions();
            this.GridOptions.GroupHeaderRows = this.GroupHeaderRows();
            this.GridOptions.GroupHeaderRows[0].Columns[0].HeaderText = Labels.Site;
            this.GridOptions.GroupHeaderRows[0].Columns[0].MergeRowNumber = 1;
            this.GridOptions.CustomizeHeader = true;
            this.GridOptions.NumberOfFrozenColumn = 0;
            this.GridOptions.Columns = this.HeaderRow();
            this.GridOptions.Columns[0].HeaderText = criteria.Site == 1 ? Labels.Credit_Site : Labels.Deposit_Site;
            this.GridOptions.Columns[0].IsBold = false;
            this.GridOptions.Columns[0].CellAlignment = HAlignment.Left;
            this.GridOptions.ShowDistinctRow = true;
            this.GridOptions.ShowHighlightRow = false;
            this.GridOptions.ShowTitle = true;
            this.GridOptions.ShowFooter = true;
            this.GridOptions.TitleType = TitleType.TwoLineBlue;
            this.GridOptions.TitleInfoText = "Main > Soccer - Minor Bet Types > " +
                CacheRef.GetBettype(criteria.BetType) +
                " " +
                (criteria.LiveIndicator ? "\"Live\"" : "\"Non - Live\"")
                + " | "
                + string.Format(MainResources.DailyWinLoss2h_WinLossBySite_BetType_ReportTitle, criteria.Site == 1 ? "Credit" : "Deposit")
                + " Site";
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                this.Reader = db.ExecuteReader();
                while (this.Reader.Read())
                {
                    this.GridOptions.BodyRows.Add(this.MapSPToDataRow());
                }
            }

            this.CalculateGrandTotal();
            if (this._grandTotalRow == null)
            {
                this.GridOptions.ShowFooter = false;
            }
            else
            {
                this.GridOptions.FooterRows.Add(this._grandTotalRow);
            }

            SessionManager.Set<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorSiteSessionKey, this.GridOptions);
            return this.GridOptions;
        }

        /// <summary>
        /// Maps the SP to data row.
        /// </summary>
        /// <returns>Grid Data Row.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow MapSPToDataRow()
        {
            var gridDataRow = new GridDataRow();
            int currentColIndex = 0;
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.SportType, Value = this.Reader["site"].ToString() });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.WinLoss, Value = Converter.ToDecimal(this.Reader["WinLost"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Position, Value = Converter.ToDecimal(this.Reader["Position"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Commission, Value = Converter.ToDecimal(this.Reader["Commission"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.LineTotal + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealLineTotal"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.LineTotal + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualLineTotal"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Variance, Value = 0, CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Turnover + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealTurnOver"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Turnover + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualTurnOver"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Margin + Labels.Real, Value = 0, CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Margin + Labels.Virtual, Value = 0, CellAlignment = HAlignment.Right });
            this.RecalculateVarianceAndMarginValue(gridDataRow);

            return gridDataRow;
        }

        /// <summary>
        /// Calculates the grand total.
        /// </summary>
        private void CalculateGrandTotal()
        {
            if (this.GridOptions.BodyRows.Count == 0)
            {
                return;
            }

            if (this._grandTotalRow == null)
            {
                this._grandTotalRow = this.GridOptions.BodyRows.Count > 0 ? this.GridOptions.BodyRows[0].Clone() : new GridDataRow();
                this._grandTotalRow.Columns[0].Value = Labels.Total;
                this._grandTotalRow.Columns[0].CellAlignment = HAlignment.Right;
            }

            for (int i = 1; i < this.GridOptions.BodyRows.Count; i++)
            {
                this._grandTotalRow.AddValue(this.GridOptions.BodyRows[i]);
            }

            this.RecalculateVarianceAndMarginValue(this._grandTotalRow);
        }
    }
}
