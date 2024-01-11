namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    public class RMSection : ConfigurationSection
    {
        public enum RMType
        {
            EightRM = 0,

            BARM = 1
        }

        public static RMSection Instance
        {
            get
            {
                var storeProcedureSection =
                 ConfigurationManager.GetSection("RMSection") as RMSection;
                return storeProcedureSection;
            }
        }

        [ConfigurationProperty("Report", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RMCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RMCollection Report
        {
            get
            {
                var storeProcedureCollection =
                    (RMCollection)base["Report"];
                return storeProcedureCollection;
            }
        }
    }
}
