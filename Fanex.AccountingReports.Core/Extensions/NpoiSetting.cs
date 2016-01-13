// -----------------------------------------------------------------------
// <copyright file="NpoiSetting.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Core.Extensions
{
    using NPOI.SS.UserModel;
using System.Drawing;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NpoiSetting
    {
        public short FillForegroundColorIndex { get; set; }

        public short FontColorIndex { get; set; }

        public bool IsBold { get; set; }

        public Color BackgroundColor { get; set; }

        public HorizontalAlignment Alignment { get; set; }

        public string DataFormat { get; set; }
    }
}
