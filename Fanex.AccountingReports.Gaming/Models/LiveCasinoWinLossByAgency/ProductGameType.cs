namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency
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
