namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;
    using Fanex.Common;
    
    public class RMElement : ConfigurationElement
    {
        public RMElement()
        {
        }

        [ConfigurationProperty("Id", IsRequired = false)]
        public string Id
        {
            get
            {
                return Converter.ToString2(this["Id"]);
            }

            set
            {
                this["Id"] = value;
            }
        }

        [ConfigurationProperty("Name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }

            set
            {
                this["Name"] = value;
            }
        }

        [ConfigurationProperty("DisplayName", IsRequired = false)]
        public string DisplayName
        {
            get
            {
                return (string)this["DisplayName"];
            }

            set
            {
                this["DisplayName"] = value;
            }
        }

        [ConfigurationProperty("IsActive", IsRequired = false)]
        public bool IsActive
        {
            get
            {
                return Converter.ToBool(this["IsActive"]);
            }

            set
            {
                this["IsActives"] = value;
            }
        }

        [ConfigurationProperty("ReportConfigs", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RMCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RMCollection ReportConfigs
        {
            get
            {
                var stores = (RMCollection)base["ReportConfigs"];
                return stores;
            }
        }
    }
}
