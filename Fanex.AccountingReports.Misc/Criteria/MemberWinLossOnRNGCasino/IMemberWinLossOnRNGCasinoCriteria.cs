// -----------------------------------------------------------------------
// <copyright file="IMemberWinLossOnRNGCasinoCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Criteria.MemberWinLossOnRNGCasino
{
    using System;
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMemberWinLossOnRNGCasinoCriteria 
    {
        double Amount { get; set; }

        DateTime FromDate { get; set; }

        DateTime ToDate { get; set; }

        bool IsAutoRefresh { get; set; }

        bool IsValid();

        string GetSettingKey();

        Hashtable GetSpParams();
    }
}
