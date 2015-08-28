// -----------------------------------------------------------------------
// <copyright file="GridDataRow.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.KenoWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Grid Data Row.
    /// </summary>
    [Serializable]
    public class GridDataRow
    {
        public GridDataRow()
        {
            this.Columns = new List<Columns>();
            this.CssClass = string.Empty;
            this.SubRows = new List<GridDataRow>();
        }

        public List<Columns> Columns { get; set; }

        public string CssClass { get; set; }

        public List<GridDataRow> SubRows { get; set; }
    }
}
