// -----------------------------------------------------------------------
// <copyright file="GridOptions.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.KenoWinLoss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft;
    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class GridOptions
    {
        public GridOptions()
        {
            this.Columns = new List<HeaderColumns>();
            this.BodyRows = new List<GridDataRow>();
            this.FooterRows = new List<GridDataRow>();
        }

        [JsonProperty("columns")]
        public List<HeaderColumns> Columns { get; set; }

        [JsonProperty("bodyRows")]
        public List<GridDataRow> BodyRows { get; set; }

        [JsonProperty("footerRows")]
        public List<GridDataRow> FooterRows { get; set; }

        [JsonProperty("showTitle")]
        public bool ShowTitle { get; set; }

        public string TitleInfoText { get; set; }

        [JsonProperty("showFooter")]
        public bool ShowFooter { get; set; }

        [JsonProperty("noDataText")]
        public string NoDataText { get; set; }

        [JsonProperty("showDistinctRow")]
        public bool ShowDistinctRow { get; set; }

        [JsonProperty("showHighlightRow")]
        public bool ShowHighlightRow { get; set; }
    }
}
