// --------------------------------------------------------------------------
// <copyright file="GetReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.BingoOutStanding
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "Username")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ExcludeTest")]
        public bool ExcludeTestAccounts { get; set; }

        [SpParam(SqlParamName = "Site")]
        public string LicenseeList { get; set; }

        [SpParam(SqlParamName = "Currency")]
        public string Currency { get; set; }

        [SpParam(Ignored = true)]
        public bool IsChangedValue { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_BingoOutStanding_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
