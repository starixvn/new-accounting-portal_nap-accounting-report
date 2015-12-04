namespace Fanex.AccountingReports.Gaming.Models.NumberGameFinancialReport
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductGameType
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public List<BaseModel> GameTypes { get; set; }
    }
}