namespace Fanex.AccountingReports.Main.Models.CreditTerm
{
    using System;

    [Serializable]
    public class EditedRemarkResult
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string AccountEditedRemarkDate { get; set; }
    }
}
