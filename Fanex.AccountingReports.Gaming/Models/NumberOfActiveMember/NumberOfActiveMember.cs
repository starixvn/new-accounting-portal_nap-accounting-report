namespace Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember
{
    using System;

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
