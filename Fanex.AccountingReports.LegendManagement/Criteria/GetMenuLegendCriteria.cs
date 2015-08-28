// --------------------------------------------------------------------------
// <copyright file="GetMenuLegendCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get Menu Legend Criteria.
    /// </summary>
    public class GetMenuLegendCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public int MenuLegendId { get; set; }

        public string ConfigPath { get; set; }

        public string FunctionName { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "SelMenuLegend";
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
