namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using System;
    using Fanex.Data.Repository;

    public class GetRemarkEditedCriteria : CriteriaBase
    {
        public string AccountRemark { get; set; }

        public int CustID { get; set; }

        public int UserID { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string Language { get; set; }

        public DateTime AccountRemarkDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CreditTermUpdRemark";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
