namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalanceMemberMailFormat
    {
        public string Site { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }
    }
}
