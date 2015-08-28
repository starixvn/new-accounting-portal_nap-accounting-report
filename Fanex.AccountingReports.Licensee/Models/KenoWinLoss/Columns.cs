// -----------------------------------------------------------------------
// <copyright file="Columns.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.KenoWinLoss
{
    using System;

    /// <summary>
    /// The Columns.
    /// </summary>
    [Serializable]
    public class Columns
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public int MergeRowNumber { get; set; }

        public int MergeColNumber { get; set; }

        public string TdClass { get; set; }

        public bool ShouldSerializeMergeRowNumber()
        {
            return MergeRowNumber != 0;
        }

        public bool ShouldSerializeMergeColNumber()
        {
            return MergeColNumber != 0;
        }

        public bool ShouldSerializeTdClass()
        {
            return !string.IsNullOrEmpty(TdClass);
        }
    }
}
