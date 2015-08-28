// -----------------------------------------------------------------------
// <copyright file="ProductGameType.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductGameType
    {
        public string ProductName { get; set; }

        public List<EntityBase<int, string>> GameTypes { get; set; }
    }
}