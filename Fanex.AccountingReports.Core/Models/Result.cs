namespace Fanex.AccountingReports.Core.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class Result
    {
        public Result()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
