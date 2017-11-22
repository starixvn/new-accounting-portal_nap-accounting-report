namespace Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NumberOfActiveMember
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public int MemberCount { get; set; }

        public string GameName { get; set; }

        public string ProductId { get; set; }
    }
}
