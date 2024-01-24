namespace Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser
{
    using System;
    using Fanex.Data.Repository;

    public class PeakConcurrentUserCriteria : CriteriaBase, ICriteria
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string LicenseeName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_PeakConcurrentUser_GetReport";
        }
    }
}
