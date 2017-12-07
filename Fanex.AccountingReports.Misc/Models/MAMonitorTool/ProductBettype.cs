namespace Fanex.AccountingReports.Misc.Models.MAMonitorTool
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProductBettype
    {
        public ProductBettype()
        {
            Bettypes = new List<Bettype>();
        }

        public string ProductName { get; set; }

        public List<Bettype> Bettypes { get; set; }
    }
}
