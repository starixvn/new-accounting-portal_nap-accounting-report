namespace Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers
{
    using System;

    [Serializable]
    public class DetailInformation
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string CreatedDate { get; set; }

        public string Creator { get; set; }
    }
}
