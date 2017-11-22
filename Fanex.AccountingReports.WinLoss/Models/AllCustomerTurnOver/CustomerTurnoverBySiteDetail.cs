namespace Fanex.AccountingReports.WinLoss.Models.AllCustomerTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Customer turnover by site .
    /// </summary>
    [Serializable]
    public class CustomerTurnoverBySiteDetail
    {
        public CustomerTurnoverBySiteDetail()
        {
            Name = string.Empty;
            Rows = new List<CustomerTurnoverBySiteDetail>();
        }

        public string Name { get; set; }

        public decimal Turnover { get; set; }

        public List<CustomerTurnoverBySiteDetail> Rows { get; set; }

        public void SumTotal(CustomerTurnoverBySiteDetail site)
        {
            this.Turnover += site.Turnover;
        }
    }
}
