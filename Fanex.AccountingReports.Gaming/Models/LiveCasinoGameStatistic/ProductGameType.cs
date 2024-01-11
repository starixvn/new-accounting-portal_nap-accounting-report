namespace Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    public class ProductGameType
    {
        public string ProductName { get; set; }

        public List<EntityBase<int, string>> GameTypes { get; set; }
    }
}
