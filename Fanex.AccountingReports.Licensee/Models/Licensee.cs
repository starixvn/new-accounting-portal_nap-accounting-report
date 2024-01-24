namespace Fanex.AccountingReports.Licensee.Models
{
    using System;

    [Serializable]
    public class Licensee
    {
        public int ResellerId { get; set; }

        public string ResellerName { get; set; }

        public int ResellerType { get; set; }

        public int LicenseeId { get; set; }

        public string LicenseeName { get; set; }

        public bool IsEnabledBA { get; set; }

        public string BrandName { get; set; }
        
        public bool IsMultiAgent { get; set; }
    }
}
