namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NegativeBalanceMemberSendMailFail
    {
        public NegativeBalanceMemberSendMailFail()
        {
            Licensees = new List<NegativeBalanceMemberSendMailResult>();
        }

        public bool IsFail { get; set; }

        public List<NegativeBalanceMemberSendMailResult> Licensees { get; set; }
    }
}
