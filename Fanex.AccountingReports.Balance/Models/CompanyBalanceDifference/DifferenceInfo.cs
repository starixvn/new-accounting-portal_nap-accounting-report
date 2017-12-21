namespace Fanex.AccountingReports.Balance.Models.CompanyBalanceDifference
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DifferenceInfo
    {
        public string BalanceUpDate { get; set; }

        public decimal BalanceBeforeBalanceUpDate { get; set; }

        public decimal BalanceAfterBalanceUpDate { get; set; }

        public decimal Difference { get; set; }
    }
}