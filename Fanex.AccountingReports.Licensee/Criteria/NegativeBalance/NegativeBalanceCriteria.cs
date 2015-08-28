// --------------------------------------------------------------------------
// <copyright file="NegativeBalanceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.NegativeBalance
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NegativeBalanceCriteria : CriteriaBase
    {
        public string Site { get; set; }

        [SpParam(Ignored = true)]
        public bool IsGetCache { get; set; }

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
            return "Licensee_NegativeBalance_GetReport";
        }
    }
}
