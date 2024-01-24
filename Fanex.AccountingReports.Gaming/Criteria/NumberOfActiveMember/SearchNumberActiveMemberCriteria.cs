namespace Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember
{
    using System;

    public class SearchNumberActiveMemberCriteria
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ProductList { get; set; }

        public string ProductName { get; set; }
    }
}
