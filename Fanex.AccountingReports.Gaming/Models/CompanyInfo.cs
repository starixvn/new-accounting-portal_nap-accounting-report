namespace Fanex.AccountingReports.Gaming.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CompanyInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string EffectiveDate { get; set; }

        public List<CompanyInfo> ChildInfo { get; set; }
    }
}
