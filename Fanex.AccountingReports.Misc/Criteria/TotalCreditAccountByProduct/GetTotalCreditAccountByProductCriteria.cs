// -----------------------------------------------------------------------
// <copyright file="GetTotalCreditAccountByProductCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get total credit account by product criteria.
    /// </summary>
    public class GetTotalCreditAccountByProductCriteria : CriteriaBase
    {
        public string ListCurrency { get; set; }

        public bool ShowEnabled { get; set; }

        public bool ShowDisabled { get; set; }

        public int SportType { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_EnableDisableAccount_ByProduct";
        }
    }
}