namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Number Game Financial Criteria.
    /// </summary>
    public class GetNumberGameFinancialCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "datestart")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "dateend")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Number_Game_Financial_Report";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}
