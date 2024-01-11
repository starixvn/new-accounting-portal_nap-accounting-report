namespace Fanex.AccountingReports.Gaming.Core.Configurations.VirtualSportsWinLossSection
{
    using System.Configuration;

    public class VirtualSportsWinLossSection : ConfigurationSection
    {
        public static VirtualSportsWinLossSection Instance 
        {
            get
            {
                var storeSection =
                    ConfigurationManager.GetSection("VirtualSportsWinLossSection") as VirtualSportsWinLossSection;
                return storeSection;
            }
        }

        [ConfigurationProperty("SportsType", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(VirtualSportsCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public VirtualSportsCollection VirtualSportsGroup
        {
            get
            {
                var sportsCollection = (VirtualSportsCollection)base["SportsType"];
                return sportsCollection;
            }
        }
    }
}
