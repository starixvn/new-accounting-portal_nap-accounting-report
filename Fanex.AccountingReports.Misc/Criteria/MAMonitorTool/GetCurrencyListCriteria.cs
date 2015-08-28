// --------------------------------------------------------------------------
// <copyright file="GetCurrencyListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Currency List Criteria.
    /// </summary>
    public class GetCurrencyListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "system")]
        public string SystemName
        {
            get
            {
                return SystemNameConstants.Accounting;
            }
        }

        public override string GetSettingKey()
        {
            return "NAP_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
