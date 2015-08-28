// --------------------------------------------------------------------------
// <copyright file="GetLevelReportByCustIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLevelReportByCustIdCriteria : WinLossByLicenseeCriteria
    {
        public int Srecommend
        {
            get { return CustID; }
        }

        [SpParam(SqlParamName = "mrecommend")]
        public int MRecommend
        {
            get { return CustID; }
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            string alias = string.Empty;
            switch (this.LevelReport)
            {
                case 1:
                    alias = "Licensee_WinLossByLicensee_GetWinLossSuper";
                    break;
                case 2:
                    alias = "Licensee_WinLossByLicensee_GetWinLossMaster";
                    break;
                case 3:
                    alias = "Licensee_WinLossByLicensee_GetWinLossAgent";
                    break;
                case 4:
                    alias = "Licensee_WinLossByLicensee_GetWinLossMember";
                    break;
                default:
                    alias = string.Empty;
                    break;
            }

            return alias;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
