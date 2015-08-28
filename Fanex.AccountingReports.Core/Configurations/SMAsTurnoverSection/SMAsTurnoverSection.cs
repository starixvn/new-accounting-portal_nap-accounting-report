// -----------------------------------------------------------------------
// <copyright file="SMAsTurnoverSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.05">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SMAsTurnoverSection : ConfigurationSection
    {
        public static SMAsTurnoverSection Instance
        {
            get
            {
                var smaTurnover = ConfigurationManager.GetSection("SMAsTurnoverSection") as SMAsTurnoverSection;
                return smaTurnover;
            }
        }

        [ConfigurationProperty("AccountStatus", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MAMonitorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public SMAsTurnoverCollection AccountStatus
        {
            get
            {
                var accountStatus = (SMAsTurnoverCollection)base["AccountStatus"];
                return accountStatus;
            }
        }

        [ConfigurationProperty("Products", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MAMonitorCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public SMAsTurnoverCollection Products
        {
            get
            {
                var products = (SMAsTurnoverCollection)base["Products"];
                return products;
            }
        }
    }
}