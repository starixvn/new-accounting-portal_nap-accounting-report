// --------------------------------------------------------------------------
// <copyright file="LicenseeReportSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeReportSection : ConfigurationSection
    {
        public static LicenseeReportSection Instance
        {
            get
            {
                var storeProcedureSection =
                 ConfigurationManager.GetSection("LicenseeReportSection") as LicenseeReportSection;
                return storeProcedureSection;
            }
        }

        [ConfigurationProperty("LicenseesGroup", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(LicenseeReportCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public LicenseeReportCollection LicenseesGroup
        {
            get
            {
                var sportsCollection =
                    (LicenseeReportCollection)base["LicenseesGroup"];
                return sportsCollection;
            }
        }
    }
}