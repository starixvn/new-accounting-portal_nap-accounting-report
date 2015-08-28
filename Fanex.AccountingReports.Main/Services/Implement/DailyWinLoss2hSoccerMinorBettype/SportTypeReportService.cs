// -----------------------------------------------------------------------
// <copyright file="SportTypeReportService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Enums.DailyWinloss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportTypeReportService : BaseReportService, ISoccerReportService
    {
        private Dictionary<object, string> _bettypes;

        private GridOptions _liveReport;

        private GridOptions _nonliveReport;

        private List<GridDataRow> _liveGrandTotal;

        private List<GridDataRow> _nonliveGrandTotal;

        private int _groupIndex;

        public SportTypeReportService()
        {
            this._bettypes = new Dictionary<object, string>();
            this._liveGrandTotal = new List<GridDataRow>();
            this._nonliveGrandTotal = new List<GridDataRow>();
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isCached">If set to <c>true</c> [is cached].</param>
        /// <returns>
        /// List Grid Options.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<GridOptions> GetReport(GetSoccerReportCriteria criteria, bool isCached = false)
        {
            List<GridOptions> result = new List<GridOptions>();
            if (isCached)
            {
                result = SessionManager.Get<List<GridOptions>>(MainConstants.DailyWinLoss2hSoccerMinorSporttypeSessionKey);
                if (result != null)
                {
                    return result;
                }
            }

            Init();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                this.Reader = db.ExecuteReader();
                List<GridDataRow> blockData = new List<GridDataRow>();
                int previousBettypeID = -1;
                string previousHedge = string.Empty;
                bool isLive = false;
                while (this.Reader.Read())
                {
                    string currentHedge = Convert.ToString(this.Reader["isHedge"]);
                    int currentBettypeID = Convert.ToInt32(this.Reader["bettype"]);
                    if (!this._bettypes.ContainsKey(currentBettypeID))
                    {
                        this._bettypes.Add(currentBettypeID, CacheRef.GetBettype(currentBettypeID));
                    }

                    if (currentBettypeID != previousBettypeID && previousBettypeID != -1)
                    {
                        var bettypeTotalRow = CalculateBettypeTotalRow(blockData, previousBettypeID, previousHedge);
                        blockData.Insert(0, bettypeTotalRow);
                        foreach (GridDataRow row in blockData)
                        {
                            if (isLive)
                            {
                                this._liveReport.BodyRows.Add(row);
                            }
                            else
                            {
                                this._nonliveReport.BodyRows.Add(row);
                            }
                        }

                        this.CalculateGrandTotalRow(blockData, isLive);
                        blockData = new List<GridDataRow>();
                        this._groupIndex++;
                    }

                    isLive = Converter.ToBool(this.Reader["liveindicator"]) ||
                        Converter.ToInt(this.Reader["liveindicator"]) == 1;
                    blockData.Add(this.MapSPToDataRow());
                    previousBettypeID = currentBettypeID;
                    previousHedge = currentHedge;
                }

                if (blockData != null && blockData.Count != 0)
                {
                    var lastBettypeTotalRow = CalculateBettypeTotalRow(blockData, previousBettypeID, previousHedge);
                    blockData.Insert(0, lastBettypeTotalRow);
                    foreach (GridDataRow row in blockData)
                    {
                        if (isLive)
                        {
                            this._liveReport.BodyRows.Add(row);
                        }
                        else
                        {
                            this._nonliveReport.BodyRows.Add(row);
                        }
                    }

                    this.CalculateGrandTotalRow(blockData, isLive);
                    this.Map8RMAndBARMGrandTotalRows();
                }
            }

            this._liveReport.FooterRows = this._liveGrandTotal;
            this._nonliveReport.FooterRows = this._nonliveGrandTotal;
            result.Add(this._nonliveReport);
            result.Add(this._liveReport);
            SessionManager.Set<List<GridOptions>>(MainConstants.DailyWinLoss2hSoccerMinorSporttypeSessionKey, result);
            return result;
        }

        protected override List<GroupHeaderRows> GroupHeaderRows()
        {
            var result = base.GroupHeaderRows();
            result[0].Columns[0].HeaderText = Labels.Bet_Type;
            result[0].Columns[0].Name = Labels.Bettype;
            return result;
        }

        protected override List<HeaderColumns> HeaderRow()
        {
            var result = base.HeaderRow();
            result[0].HeaderText = Labels.Bet_Type;
            result[0].Name = Labels.Bettype;
            return result;
        }

        /// <summary>
        /// Map8s the RM and BARM grand total rows.
        /// </summary>
        /// <returns>Is live value.</returns>
        private bool Map8RMAndBARMGrandTotalRows()
        {
            bool isLive = true;
            this.Reader.NextResult();
            while (this.Reader.Read())
            {
                isLive = Converter.ToBool(this.Reader["liveindicator"]) ||
                        Converter.ToInt(this.Reader["liveindicator"]) == 1;
                if (isLive)
                {
                    this._liveGrandTotal.Add(this.GetWinLoss8RMRow());
                }
                else
                {
                    this._nonliveGrandTotal.Add(this.GetWinLoss8RMRow());
                }
            }

            this.Reader.NextResult();
            while (this.Reader.Read())
            {
                isLive = Converter.ToBool(this.Reader["liveindicator"]) ||
                        Converter.ToInt(this.Reader["liveindicator"]) == 1;
                if (isLive)
                {
                    this._liveGrandTotal.Add(this.GetWinLossBARMRow());
                }
                else
                {
                    this._nonliveGrandTotal.Add(this.GetWinLossBARMRow());
                }
            }

            return isLive;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            this._liveReport = new GridOptions();
            this._liveReport.GroupHeaderRows = this.GroupHeaderRows();
            this._liveReport.CustomizeHeader = true;
            this._liveReport.Columns = this.HeaderRow();
            this._liveReport.ShowDistinctRow = false;
            this._liveReport.ShowHighlightRow = false;
            this._liveReport.ShowTitle = true;
            this._liveReport.ShowFooter = true;
            this._liveReport.TitleInfoText = "SOCCER \"LIVE\" PRODUCT WIN/LOSS REPORT";
            this._liveReport.TitleType = TitleType.OneLineBlue;
            this._liveReport.TitleSeperatorRowNum = 0;
            this._nonliveReport = new GridOptions();
            this._nonliveReport.GroupHeaderRows = this.GroupHeaderRows();
            this._nonliveReport.CustomizeHeader = true;
            this._nonliveReport.Columns = this.HeaderRow();
            this._nonliveReport.ShowDistinctRow = false;
            this._nonliveReport.ShowHighlightRow = false;
            this._nonliveReport.ShowTitle = true;
            this._nonliveReport.ShowFooter = true;
            this._nonliveReport.TitleInfoText = "SOCCER \"NONE - LIVE\" PRODUCT WIN/LOSS REPORT";
            this._nonliveReport.TitleType = TitleType.OneLineBlue;
            this._nonliveReport.TitleSeperatorRowNum = 0;
        }

        /// <summary>
        /// Maps the SP to data row.
        /// </summary>
        /// <returns>The Data row.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow MapSPToDataRow()
        {
            GridDataRow gridDataRow = new GridDataRow();
            int currentColIndex = 0;
            int site = Convert.ToInt32(this.Reader["Site"]);
            int bettype = Converter.ToInt(this.Reader["bettype"]);
            string bettypeName = this._bettypes[bettype];
            bool isLive = Converter.ToBool(this.Reader["liveindicator"]) || Converter.ToInt(this.Reader["liveindicator"]) == 1;
            var customAttr = "bettype=" + bettype.ToString() + " liveindicator=" + (isLive ? "1" : "0") + " ishedge='" + this.Reader["isHedge"].ToString() + "' bettypename='" + bettypeName + "'";
            gridDataRow.Columns.Add(new Columns(currentColIndex++) { Name = Labels.Bettype, Value = site == 1 ? Labels.Credit : Labels.Deposit, CellAlignment = HAlignment.Right, Bold = true, CustomAttributes = customAttr });
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
            gridDataRow.CssClass += " data-group-" + this._groupIndex.ToString();
            gridDataRow.CssClass = gridDataRow.CssClass.Trim();
            return gridDataRow;
        }

        /// <summary>
        /// Calculates the bet type total row.
        /// </summary>
        /// <param name="blockData">The block data.</param>
        /// <param name="previousBettypeID">The previous bet type identifier.</param>
        /// <param name="hedge">The hedge.</param>
        /// <returns>Bet type total row.</returns>
        private GridDataRow CalculateBettypeTotalRow(List<GridDataRow> blockData, int previousBettypeID, string hedge)
        {
            var bettypeTotalRow = blockData[0].Clone();
            bettypeTotalRow.IsEven = true;
            bettypeTotalRow.CssClass = "freegrid-even";
            bettypeTotalRow.Columns[0].CellAlignment = HAlignment.Left;
            bettypeTotalRow.Columns[0].Value = this.GetBetTypeName(previousBettypeID, hedge);
            bettypeTotalRow.Columns[0].TDClass = "bettypeID-" + previousBettypeID.ToString();
            if (blockData.Count == 2)
            {
                bettypeTotalRow.AddValue(blockData[1]);
            }

            this.RecalculateVarianceAndMarginValue(bettypeTotalRow);
            return bettypeTotalRow;
        }

        /// <summary>
        /// Calculates the grand total row.
        /// </summary>
        /// <param name="blockData">The block data.</param>
        /// <param name="isLive">If set to <c>true</c> [is live].</param>
        private void CalculateGrandTotalRow(List<GridDataRow> blockData, bool isLive)
        {
            var grandTotal = isLive ? this._liveGrandTotal : this._nonliveGrandTotal;
            if (grandTotal.Count == 0)
            {
                grandTotal.Add(blockData[0].Clone()); // Total
                grandTotal.Add(blockData[0].Clone()); // Credit
                grandTotal.Add(blockData[0].Clone()); // Deposit
                grandTotal[0].Columns[0].Value = Labels.All_Total;
                grandTotal[0].Columns[0].CellAlignment = HAlignment.Right;
                grandTotal[1].Columns[0].Value = Labels.Credit;
                grandTotal[1].Columns[0].CellAlignment = HAlignment.Right;
                grandTotal[1].CssClass = "data-group-" + (isLive ? "9999" : "9998");
                grandTotal[2].Columns[0].Value = Labels.Deposit;
                grandTotal[2].Columns[0].CellAlignment = HAlignment.Right;
                grandTotal[2].CssClass = "data-group-" + (isLive ? "9999" : "9998");
                this.ResetValue(grandTotal[0]);
                this.ResetValue(grandTotal[1]);
                this.ResetValue(grandTotal[2]);
            }

            foreach (GridDataRow row in blockData)
            {
                if (row.Columns[0].Value.ToString().Equals(Labels.Credit))
                {
                    grandTotal[1].AddValue(row);
                    this.RecalculateVarianceAndMarginValue(grandTotal[1]);
                }
                else if (row.Columns[0].Value.ToString().Equals(Labels.Deposit))
                {
                    grandTotal[2].AddValue(row);
                    this.RecalculateVarianceAndMarginValue(grandTotal[2]);
                }
                else
                {
                    grandTotal[0].AddValue(row);
                    this.RecalculateVarianceAndMarginValue(grandTotal[0]);
                }
            }

            grandTotal[0].Columns[(int)ColumnMappingEnums.LineTotalReal].CustomCellType = ExcelCellType.DoubleRedPositiveNoteInformation;
            grandTotal[0].Columns[(int)ColumnMappingEnums.LineTotalReal].TDClass += " colNote";
            grandTotal[0].Columns[(int)ColumnMappingEnums.LineTotalVirtual].CustomCellType = ExcelCellType.DoubleRedPositiveNoteInformation;
            grandTotal[0].Columns[(int)ColumnMappingEnums.LineTotalVirtual].TDClass += " colNote";
            grandTotal[0].Columns[(int)ColumnMappingEnums.Variance].CustomCellType = ExcelCellType.DoubleRedPositiveNoteInformation;
            grandTotal[0].Columns[(int)ColumnMappingEnums.Variance].TDClass += " colNote";
        }

        /// <summary>
        /// Gets the win loss 8RM row.
        /// </summary>
        /// <returns>The win loss 8RM data.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow GetWinLoss8RMRow()
        {
            int columnIndex = 0;
            var winloss8RM = new GridDataRow();
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Bettype, CellAlignment = HAlignment.Right, Value = Labels._8RM + " " + Labels.Total });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.WinLoss, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["WinLost"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Position, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["Position"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Commission, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["Commission"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Real, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["RealLineTotal"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Virtual, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["VirtualLineTotal"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Variance, CellAlignment = HAlignment.Right, Value = 0 });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Real, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["RealTurnover"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Virtual, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["VirtualTurnover"]) });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Margin + Labels.Real, Value = 0, CellAlignment = HAlignment.Right });
            winloss8RM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Margin + Labels.Virtual, Value = 0, CellAlignment = HAlignment.Right });
            this.RecalculateVarianceAndMarginValue(winloss8RM);
            return winloss8RM;
        }

        /// <summary>
        /// Gets the win loss BARM row.
        /// </summary>
        /// <returns>The win loss BARM data.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private GridDataRow GetWinLossBARMRow()
        {
            int columnIndex = 0;
            var winlossBARM = new GridDataRow();
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Bettype, CellAlignment = HAlignment.Right, Value = Labels.BARM + " " + Labels.Total });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.WinLoss, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["WinLost"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Position, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["Position"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Commission, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["Commission"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Real, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["RealLineTotal"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.LineTotal + Labels.Virtual, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["VirtualLineTotal"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Variance, CellAlignment = HAlignment.Right, Value = 0 });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Real, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["RealTurnover"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Turnover + Labels.Virtual, CellAlignment = HAlignment.Right, Value = Converter.ToDecimal(this.Reader["VirtualTurnover"]) });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Margin + Labels.Real, Value = 0, CellAlignment = HAlignment.Right });
            winlossBARM.Columns.Add(new Columns(columnIndex++) { Name = Labels.Margin + Labels.Virtual, Value = 0, CellAlignment = HAlignment.Right });
            this.RecalculateVarianceAndMarginValue(winlossBARM);
            return winlossBARM;
        }

        /// <summary>
        /// Resets the value.
        /// </summary>
        /// <param name="row">The data row.</param>
        private void ResetValue(GridDataRow row)
        {
            row.Columns[(int)ColumnMappingEnums.WinLoss].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.Position].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.Commission].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.LineTotalReal].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.LineTotalVirtual].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.Variance].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.TurnoverReal].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.TurnoverVirtual].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.MarginReal].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.MarginVirtual].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.WinLoss].Value = 0.0;
            row.Columns[(int)ColumnMappingEnums.WinLoss].Value = 0.0;
        }

        /// <summary>
        /// Gets the name of the bet type.
        /// </summary>
        /// <param name="bettype">The bet type.</param>
        /// <param name="hedge">The hedge.</param>
        /// <returns>
        /// The name of the bet type.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private string GetBetTypeName(int bettype, string hedge)
        {
            string result = string.Empty;
            switch (bettype)
            {
                case CoreConstants.BetTypeConstants.Handicap:
                    result = Labels.HDP;
                    break;
                case CoreConstants.BetTypeConstants.OverUnder2:
                    result = Labels.OU;
                    break;
                case CoreConstants.BetTypeConstants._1stHandicap:
                    result = Labels._1st_HDP;
                    break;
                case CoreConstants.BetTypeConstants._1stOverUnder:
                    result = Labels._1st_OU;
                    break;
                case CoreConstants.BetTypeConstants.CorrectScore:
                    result = Labels.CS;
                    break;
                case CoreConstants.BetTypeConstants._1stCorrectScore:
                    result = Labels._1st_CS;
                    break;
                case CoreConstants.BetTypeConstants.TotalGoal:
                    result = Labels.TG;
                    break;
                case CoreConstants.BetTypeConstants.FirstGoalLastGoal:
                    result = Labels.FG_LG;
                    break;
                case MainConstants.DailyWinLoss2hKickOffId:
                case CoreConstants.BetTypeConstants.KickOff:
                    result = Labels.Kick_Off;
                    break;
                case CoreConstants.BetTypeConstants.CasinoGames:
                    result = Labels.Casino;
                    break;
                case CoreConstants.BetTypeConstants.P2P:
                    result = Labels.P2P;
                    break;
                case BetTypeConstants._2hReportBingo:
                    result = Labels.IBCbet_Bingo;
                    break;
                case CoreConstants.BetTypeConstants.Bingo:
                    result = Labels.IBCbet_Bingo;
                    break;
            }

            if (!string.IsNullOrEmpty(hedge) &&
                hedge.Contains("(" + Labels.Hedge + ")"))
            {
                result += hedge;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = this._bettypes[bettype];
            }

            return result;
        }
    }
}
