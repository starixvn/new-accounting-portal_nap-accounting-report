// -----------------------------------------------------------------------
// <copyright file="HeaderColumns.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.KenoWinLoss
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Header Columns.
    /// </summary>
    [Serializable]
    public class HeaderColumns
    {
        public HeaderColumns()
        {
            this.Width = 150;
        }

        public string HeaderText { get; set; }

        public string Name { get; set; }

        public int Width { get; set; }

        [JsonIgnore]
        public int WidthExcel { get; set; }

        public string HeaderAlign { get; set; }

        public string CellAlign { get; set; }

        public string FormatNumber { get; set; }

        public string FormatNegative { get; set; }

        public string FormatPositive { get; set; }

        public string FormatDateTime { get; set; }

        public string CellCssClass { get; set; }
    }
}
