namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Enums.DailyWinloss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetTypeReportService : BaseReportService, IBettypeReportService
    {
        private readonly Dictionary<object, string> _teamName;

        private readonly Dictionary<object, string> _leagueName;

        private readonly Dictionary<object, List<GridDataRow>> _dataBlocks;

        private GetReportByBetTypeCriteria _criteria;

        private GridDataRow _grandTotalRow;

        public BetTypeReportService()
        {
            this._teamName = new Dictionary<object, string>();
            this._leagueName = new Dictionary<object, string>();
            this.GridOptions = new GridOptions();
            this._dataBlocks = new Dictionary<object, List<GridDataRow>>();
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isCached">If set to <c>true</c> [is cached].</param>
        /// <returns>
        /// Report data.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public GridOptions GetReport(GetReportByBetTypeCriteria criteria, bool isCached = false)
        {
            if (isCached)
            {
                this.GridOptions = SessionManager.Get<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorBettypeSessionKey);
                if (this.GridOptions != null)
                {
                    return this.GridOptions;
                }
            }

            var eightRMTotalRow = new GridDataRow();
            this._criteria = criteria;
            criteria.BetTypeName = CacheRef.GetBettype(criteria.Bettype);
            this.Init();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                this.Reader = db.ExecuteReader();
                int previousLeagueID = -1;
                string leagueName = string.Empty;
                List<GridDataRow> dataBlock = new List<GridDataRow>();
                GridDataRow leagueRow;
                while (this.Reader.Read())
                {
                    int currentLeagueID = Converter.ToInt(this.Reader["leagueid"]);
                    int currentHomeID = Converter.ToInt(this.Reader["homeid"]);
                    int currentAwayID = Converter.ToInt(this.Reader["awayid"]);
                    if (!this._leagueName.ContainsKey(currentLeagueID))
                    {
                        this._leagueName.Add(currentLeagueID, CacheRef.GetLeagueName(currentLeagueID));
                    }

                    if (!this._teamName.ContainsKey(currentHomeID))
                    {
                        this._teamName.Add(currentHomeID, CacheRef.GetTeamName(currentHomeID));
                    }

                    if (!this._teamName.ContainsKey(currentAwayID))
                    {
                        this._teamName.Add(currentAwayID, CacheRef.GetTeamName(currentAwayID));
                    }

                    if (previousLeagueID != -1 && currentLeagueID != previousLeagueID)
                    {
                        this.CalculateGrandTotalRow(dataBlock, false);
                        leagueName = this._leagueName[previousLeagueID].ToUpper();
                        leagueRow = new GridDataRow();
                        leagueRow.IsEven = true;
                        leagueRow.CssClass = "freegrid-even";
                        leagueRow.Columns.Add(new Columns(0) { Name = Labels.Code, Value = string.Empty });
                        leagueRow.Columns.Add(new Columns(1)
                        {
                            Name = Labels.Home,
                            MergeColNumber = 13,
                            Bold = true,
                            Value = leagueName,
                            CellAlignment = HAlignment.Left
                        });
                        if (dataBlock.Count > 1)
                        {
                            dataBlock.Add(this.CalculateTotalRow(dataBlock));
                        }

                        dataBlock.Insert(0, leagueRow);
                        this._dataBlocks.Add(leagueName, dataBlock);
                        dataBlock = new List<GridDataRow>();
                    }

                    dataBlock.Add(this.MapSPToDataRow());
                    previousLeagueID = currentLeagueID;
                }

                if (dataBlock.Count > 0)
                {
                    this.CalculateGrandTotalRow(dataBlock, true);
                    leagueName = this._leagueName[previousLeagueID].ToUpper();
                    leagueRow = new GridDataRow();
                    leagueRow.IsEven = true;
                    leagueRow.CssClass = "freegrid-even";
                    leagueRow.Columns.Add(new Columns(0) { Name = Labels.Code, Value = string.Empty });
                    leagueRow.Columns.Add(new Columns(1)
                    {
                        Name = Labels.Home,
                        MergeColNumber = 13,
                        CellAlignment = HAlignment.Left,
                        Bold = true,
                        Value = this._leagueName[previousLeagueID].ToUpper()
                    });
                    if (dataBlock.Count > 1)
                    {
                        dataBlock.Add(this.CalculateTotalRow(dataBlock));
                    }

                    dataBlock.Insert(0, leagueRow);
                    this._dataBlocks.Add(leagueName, dataBlock);

                    foreach (var block in this._dataBlocks.OrderBy(i => i.Key))
                    {
                        foreach (var row in block.Value)
                        {
                            this.GridOptions.BodyRows.Add(row);
                        }
                    }
                }

                if (this.Reader.NextResult())
                {
                    while (this.Reader.Read())
                    {
                        var _8RMTotalRow = MapRMTotalRow();
                        eightRMTotalRow = _8RMTotalRow.Clone();
                    }
                }

                if (this.Reader.NextResult())
                {
                    while (this.Reader.Read())
                    {
                        var _barmTotalRow = MapRMTotalRow();
                        eightRMTotalRow.AddValue(_barmTotalRow);
                    }

                    this.CalculateVarianceAndMarginRMDataRow(eightRMTotalRow);
                }

                this.Reader.Close();
            }

            if (this.GridOptions.BodyRows.Count > 0)
            {
                this.GridOptions.FooterRows.Add(this._grandTotalRow);
                if (eightRMTotalRow != null && Converter.ToDecimal(eightRMTotalRow.Columns[1].Value) != 0)
                {
                    this.GridOptions.FooterRows.Add(eightRMTotalRow);
                }
            }

            SessionManager.Set<GridOptions>(MainConstants.DailyWinLoss2hSoccerMinorBettypeSessionKey, this.GridOptions);
            return this.GridOptions;
        }

        /// <summary>
        /// Recalculates the variance and margin value.
        /// </summary>
        /// <param name="row">The data row.</param>
        protected override void RecalculateVarianceAndMarginValue(GridDataRow row)
        {
            int shiftColumn = this._criteria.Bettype == 10 ? 1 : 3;
            double realLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalReal + shiftColumn].Value), 2);
            double virtualLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalVirtual + shiftColumn].Value), 2);
            double realTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverReal + shiftColumn].Value), 2);
            double virtualTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverVirtual + shiftColumn].Value), 2);
            if (Math.Abs(realTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal + shiftColumn].Value = Labels.NotAvailable;
                row.Columns[(int)ColumnMappingEnums.MarginReal + shiftColumn].CellAlignment = HAlignment.Right;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal + shiftColumn].Value = Converter.ToDecimal(Math.Abs(realTurnOver) < 0 ? 0 : realLineTotal / realTurnOver);
            }

            if (Math.Abs(virtualTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual + shiftColumn].Value = Labels.NotAvailable;
                row.Columns[(int)ColumnMappingEnums.MarginVirtual + shiftColumn].CellAlignment = HAlignment.Right;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual + shiftColumn].Value = Converter.ToDecimal(Math.Abs(virtualTurnOver) < 0 ? 0 : virtualLineTotal / virtualTurnOver);
            }

            row.Columns[(int)ColumnMappingEnums.Variance + shiftColumn].Value = Converter.ToDecimal(realLineTotal - virtualLineTotal);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private void Init()
        {
            this.GridOptions = new GridOptions();
            this.GridOptions.GroupHeaderRows = this.GroupHeaderRows();
            this.GridOptions.GroupHeaderRows[0].Columns[0].HeaderText = Labels.Code;
            this.GridOptions.GroupHeaderRows[0].Columns[0].Name = Labels.Code;
            this.GridOptions.GroupHeaderRows[0].Columns.Insert(1, new HeaderColumns(true) { HeaderText = Labels.Event, Name = Labels.Home, MergeRowNumber = 2, MergeColNumber = 3 });
            this.GridOptions.CustomizeHeader = true;
            this.GridOptions.Columns = this.HeaderRow();
            this.GridOptions.Columns[0].HeaderText = Labels.Code;
            this.GridOptions.Columns[0].Name = Labels.Code;
            this.GridOptions.Columns[0].Width = 120;
            this.GridOptions.Columns.Insert(1, new HeaderColumns() { HeaderText = Labels.Away, Name = Labels.Away, CellAlignment = HAlignment.Left, Width = 200 });
            this.GridOptions.Columns.Insert(1, new HeaderColumns() { HeaderText = Labels.Versus, Name = Labels.Versus, Width = 25 });
            this.GridOptions.Columns.Insert(1, new HeaderColumns() { HeaderText = Labels.Home, Name = Labels.Home, CellAlignment = HAlignment.Right, Width = 200 });
            this.GridOptions.ShowDistinctRow = false;
            this.GridOptions.ShowHighlightRow = false;
            this.GridOptions.ShowTitle = true;
            this.GridOptions.ShowFooter = true;
            this.GridOptions.TitleInfoText = "Main>Soccer>" + this._criteria.BetTypeName + " " + (this._criteria.Liveindicator ? "\"LIVE\"" : "\"NON - LIVE\"");
            this.GridOptions.TitleType = TitleType.TwoLineBlue;
        }

        /// <summary>
        /// Maps the SP to data row.
        /// </summary>
        /// <returns>Data rows.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow MapSPToDataRow()
        {
            GridDataRow gridDataRow = new GridDataRow();
            int currentColIndex = 0;
            int leagueid = Converter.ToInt(this.Reader["leagueid"]);
            string matchcode = this.Reader["matchcode"].ToString();
            int matchid = Converter.ToInt(this.Reader["matchid"]);
            int homeid = Converter.ToInt(this.Reader["homeid"]);
            int awayid = Converter.ToInt(this.Reader["awayid"]);
            string customAttr = "onclick=\"scoreInfoManager.GetReport('" + this._criteria.Winlostdate.ToString(DateTimeFormatConstants.ShortDate) + "',";
            customAttr += "1,";
            customAttr += "'" + this._criteria.SportTypeName + "',";
            customAttr += this._criteria.Bettype + ",";
            customAttr += "'" + this._criteria.BetTypeName + "',";
            customAttr += (this._criteria.Liveindicator ? "1" : "0") + ",";
            customAttr += "'',";
            customAttr += leagueid + ",";
            customAttr += "'" + this._leagueName[leagueid] + "',";
            customAttr += matchid + ",";
            customAttr += "'" + matchcode + "',";
            customAttr += homeid + ",";
            customAttr += "'" + this._teamName[homeid] + "',";
            customAttr += awayid + ",";
            customAttr += "'" + this._teamName[awayid] + "'";
            customAttr += ")\"";
            bool isOutRightBettype = this._criteria.Bettype == BetTypeConstants.Outright && this._criteria.BetTypeName.ToLower() == "outright";

            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Code, Value = matchcode, CellAlignment = HAlignment.Center, Bold = true, TDClass = isOutRightBettype ? "bold" : "link bold", CustomAttributes = isOutRightBettype ? string.Empty : customAttr });
            if (this._criteria.Bettype == 10)
            {
                gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Home, Value = this._teamName[homeid], CellAlignment = HAlignment.Center, MergeColNumber = 3 });
                currentColIndex += 2;
            }
            else
            {
                gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Home, Value = this._teamName[homeid], CellAlignment = HAlignment.Right });
                gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Versus, Value = Labels.Vs, CellAlignment = HAlignment.Center });
                gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Away, Value = this._teamName[awayid], CellAlignment = HAlignment.Left });
            }

            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.WinLoss, Value = Converter.ToDecimal(this.Reader["WinLost"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Position, Value = Converter.ToDecimal(this.Reader["Position"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Commission, Value = Converter.ToDecimal(this.Reader["Commission"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.LineTotal + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealLineTotal"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.LineTotal + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualLineTotal"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Variance, Value = 0, CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Turnover + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealTurnOver"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Turnover + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualTurnOver"]), CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Margin + Labels.Real, Value = 0, CellAlignment = HAlignment.Right });
            gridDataRow.Columns.Add(new Columns(currentColIndex) { Name = Labels.Margin + Labels.Virtual, Value = 0, CellAlignment = HAlignment.Right });
            this.RecalculateVarianceAndMarginValue(gridDataRow);
            return gridDataRow;
        }

        /// <summary>
        /// Calculates the total row.
        /// </summary>
        /// <param name="dataBlock">The data block.</param>
        /// <returns>Total row.</returns>
        private GridDataRow CalculateTotalRow(List<GridDataRow> dataBlock)
        {
            var totalRow = dataBlock[0].Clone();
            for (int i = 1; i < dataBlock.Count; i++)
            {
                totalRow.AddValue(dataBlock[i]);
            }

            this.RecalculateVarianceAndMarginValue(totalRow);
            totalRow.Columns[0].Value = string.Empty;
            totalRow.Columns[0].MergeColNumber = 4;
            if (this._criteria.Bettype == 10)
            {
                totalRow.Columns.RemoveAt(1);
            }
            else
            {
                totalRow.Columns.RemoveAt(3);
                totalRow.Columns.RemoveAt(2);
                totalRow.Columns.RemoveAt(1);
            }

            return totalRow;
        }

        /// <summary>
        /// Calculates the total row.
        /// </summary>
        /// <param name="dataBlock">The data block.</param>
        /// <param name="isEndBlock">If set to <c>true</c> [is end block].</param>
        private void CalculateGrandTotalRow(List<GridDataRow> dataBlock, bool isEndBlock)
        {
            int startRow = 0;
            if (this._grandTotalRow == null)
            {
                this._grandTotalRow = dataBlock[0].Clone();
                startRow++;
            }

            for (; startRow < dataBlock.Count; startRow++)
            {
                this._grandTotalRow.AddValue(dataBlock[startRow]);
            }

            if (isEndBlock)
            {
                this.RecalculateVarianceAndMarginValue(this._grandTotalRow);
                this._grandTotalRow.Columns[0].Value = (this._criteria.Liveindicator ? "<font color=\"red\">Live!</font> " : string.Empty)
                    + this._criteria.BetTypeName
                    + " "
                    + Labels.Total;
                this._grandTotalRow.Columns[0].RichTextString = (this._criteria.Liveindicator ? "Live! " : string.Empty) + this._criteria.BetTypeName + " " + Labels.Total;
                if (this._criteria.Liveindicator)
                {
                    this._grandTotalRow.Columns[0].StartIndex.Add(0);
                    this._grandTotalRow.Columns[0].EndIndex.Add(5);
                    this._grandTotalRow.Columns[0].TextFont.Add(ExcelFont.RedBold);
                }

                this._grandTotalRow.Columns[0].StartIndex.Add(6);
                this._grandTotalRow.Columns[0].EndIndex.Add(this._grandTotalRow.Columns[0].RichTextString.Length);
                this._grandTotalRow.Columns[0].TextFont.Add(ExcelFont.Bold);
                this._grandTotalRow.Columns[0].MergeColNumber = 4;
                this._grandTotalRow.Columns[0].CellAlignment = HAlignment.Right;
                this._grandTotalRow.Columns[0].TDClass = string.Empty;
                this._grandTotalRow.Columns[0].CustomAttributes = string.Empty;
                if (this._criteria.Bettype == 10)
                {
                    this._grandTotalRow.Columns.RemoveAt(1);
                }
                else
                {
                    this._grandTotalRow.Columns.RemoveAt(3);
                    this._grandTotalRow.Columns.RemoveAt(2);
                    this._grandTotalRow.Columns.RemoveAt(1);
                }
            }
        }

        /// <summary>
        /// Maps the RM total row.
        /// </summary>
        /// <returns>The grid data row.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2100:CodeLineMustNotBeLongerThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow MapRMTotalRow()
        {
            int columnIndex = 0;
            GridDataRow dataRow = new GridDataRow();
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Code, Value = Labels.IBCTotal, CellAlignment = HAlignment.Right, MergeColNumber = 4 });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.WinLoss, Value = Converter.ToDecimal(this.Reader["WinLost"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Position, Value = Converter.ToDecimal(this.Reader["Position"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Commission, Value = Converter.ToDecimal(this.Reader["Commission"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealLineTotal"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualLineTotal"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Variance, Value = 0, CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Real, Value = Converter.ToDecimal(this.Reader["RealTurnOver"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Virtual, Value = Converter.ToDecimal(this.Reader["VirtualTurnOver"]), CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex++) { Name = Labels.Margin + Labels.Real, Value = 0, CellAlignment = HAlignment.Right });
            dataRow.Columns.Add(new Columns(columnIndex) { Name = Labels.Margin + Labels.Virtual, Value = 0, CellAlignment = HAlignment.Right });
            return dataRow;
        }

        /// <summary>
        /// Calculates the variance and margin RM data row.
        /// </summary>
        /// <param name="row">The data row.</param>
        private void CalculateVarianceAndMarginRMDataRow(GridDataRow row)
        {
            double realLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalReal].Value), 2);
            double virtualLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalVirtual].Value), 2);
            double realTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverReal].Value), 2);
            double virtualTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverVirtual].Value), 2);
            if (Math.Abs(realTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal].Value = Labels.NotAvailable;
                row.Columns[(int)ColumnMappingEnums.MarginReal].CellAlignment = HAlignment.Right;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal].Value = Converter.ToDecimal(Math.Abs(realTurnOver) < 0 ? 0 : realLineTotal / realTurnOver);
            }

            if (Math.Abs(virtualTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual].Value = Labels.NotAvailable;
                row.Columns[(int)ColumnMappingEnums.MarginVirtual].CellAlignment = HAlignment.Right;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual].Value = Converter.ToDecimal(Math.Abs(virtualTurnOver) < 0 ? 0 : virtualLineTotal / virtualTurnOver);
            }

            row.Columns[(int)ColumnMappingEnums.Variance].Value = Converter.ToDecimal(realLineTotal - virtualLineTotal);
        }
    }
}
