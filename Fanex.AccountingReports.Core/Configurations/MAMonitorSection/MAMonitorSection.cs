// --------------------------------------------------------------------------
// <copyright file="MAMonitorSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MAMonitorSection : ConfigurationSection
    {
        public static MAMonitorSection Instance
        {
            get
            {
                var monitor = ConfigurationManager.GetSection("MAMonitorSection") as MAMonitorSection;
                return monitor;
            }
        }

        [ConfigurationProperty("LeagueTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MAMonitorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MAMonitorCollection LeagueTypes
        {
            get
            {
                var leagueTypes = (MAMonitorCollection)base["LeagueTypes"];
                return leagueTypes;
            }
        }

        [ConfigurationProperty("Products", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MAMonitorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MAMonitorCollection Products
        {
            get
            {
                var products = (MAMonitorCollection)base["Products"];
                return products;
            }
        }

        [ConfigurationProperty("ReportBy", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MAMonitorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MAMonitorCollection ReportBy
        {
            get
            {
                var reportByList = (MAMonitorCollection)base["ReportBy"];
                return reportByList;
            }
        }
    }
}
