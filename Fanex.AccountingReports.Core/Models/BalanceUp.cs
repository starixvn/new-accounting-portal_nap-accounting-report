namespace Fanex.AccountingReports.Core.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// BalanceUp: getting balance up day and move bet day.
    /// </summary>
    public class BalanceUp
    {
        public DateTime BalanceUpDay { get; set; }

        public DateTime MoveBetsDay { get; set; }
    }
}
