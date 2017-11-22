namespace Fanex.AccountingReports.Main.Models.CreditTerm
{
    using System;

    /// <summary>
    /// The model represented the result when edited remark.
    /// </summary>
    [Serializable]
    public class EditedRemarkResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string AccountEditedRemarkDate { get; set; }
    }
}
