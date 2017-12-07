namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of edited remark.
    /// </summary>
    public class GetRemarkEditedCriteria : CriteriaBase
    {
        public string AccountRemark { get; set; }

        /// <summary>
        /// Gets or sets the customer identity.
        /// </summary>
        /// <value>The CustomerID.</value>
        public int CustID { get; set; }

        /// <summary>
        /// Gets or sets the identity of user that edited remark.
        /// </summary>
        /// <value>The user id.</value>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public int ErrCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
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
