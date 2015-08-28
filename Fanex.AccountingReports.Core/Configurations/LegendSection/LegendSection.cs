// --------------------------------------------------------------------------
// <copyright file="LegendSection.cs" company="Nexcel Solutions Vietnam">
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
    public class LegendSection : ConfigurationSection
    {
        public static LegendSection Instance
        {
            get
            {
                var storeProcedureSection =
                 ConfigurationManager.GetSection("LegendSection") as LegendSection;
                return storeProcedureSection;
            }
        }

        [ConfigurationProperty("Websites", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LegendCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public LegendCollection Websites
        {
            get
            {
                var storeProcedureCollection =
                    (LegendCollection)base["Websites"];
                return storeProcedureCollection;
            }
        }
    }
}
