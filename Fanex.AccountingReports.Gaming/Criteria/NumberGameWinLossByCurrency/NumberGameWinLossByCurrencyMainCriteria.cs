// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLossByCurrencyMainCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2013.1.22">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByCurrency
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByCurrencyMainCriteria : CriteriaBase
    {
        public string SiteID { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        [SpParam(SqlParamName = "isLive")]
        public int IsLive { get; set; }

        public string Currency { get; set; }

        public string Bettype { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberGameWinlossByCurrency_GetReport";
        }
    }
}
