namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalanceMemberSendMailResult
    {
        public string Site { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}