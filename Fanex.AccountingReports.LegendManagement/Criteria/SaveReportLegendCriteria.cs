// --------------------------------------------------------------------------
// <copyright file="SaveReportLegendCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    /// <summary>
    /// Save Report Legend Criteria.
    /// </summary>
    public class SaveReportLegendCriteria : CriteriaBase
    {
        public SaveReportLegendCriteria()
        {
            this.System = WebSettings.System;
        }

        public int MenuId { get; set; }

        public string Legend { get; set; }

        public int LegendId { get; set; }

        public string System { get; set; }

        public bool IsNote { get; set; }

        public bool IsReviewed { get; set; }

        [SpParam(Ignored = true)]
        public string ConfigPath { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_SaveReportLegend";
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