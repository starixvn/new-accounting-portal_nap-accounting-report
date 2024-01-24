namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss
{
    using System;
    using Fanex.Data.Repository;

    public class NumberGameMonthlyWinLossCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameMonthlyWinloss";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}
