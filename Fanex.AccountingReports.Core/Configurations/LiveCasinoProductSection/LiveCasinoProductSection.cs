// -----------------------------------------------------------------------
// <copyright file="LiveCasinoProductSection.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoProductSection : ConfigurationSection
    {
        public static LiveCasinoProductSection Instance
        {
            get
            {
                var section = ConfigurationManager.GetSection("LiveCasinoProductSection") as LiveCasinoProductSection;
                return section;
            }
        }

        [ConfigurationProperty("Products", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LiveCasinoProductCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public LiveCasinoProductCollection Products
        {
            get
            {
                var products = (LiveCasinoProductCollection)base["Products"];
                return products;
            }
        }
    }
}