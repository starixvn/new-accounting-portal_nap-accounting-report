// --------------------------------------------------------------------------
// <copyright file="GetInputedLegendReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get Inputted Legend Report Criteria.
    /// </summary>
    public class GetInputedLegendReportCriteria : CriteriaBase
    {
        public GetInputedLegendReportCriteria()
        {
            System = string.Empty;
        }

        public string System { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_GetInputedLegend";
        }

        /// <summary>
        /// Check data's constrains.
        /// </summary>
        /// <returns>True: valid; False: invalid.</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}