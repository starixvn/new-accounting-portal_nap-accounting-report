namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LegendElement : ConfigurationElement
    {
        public LegendElement()
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

        [ConfigurationProperty("Address", IsRequired = false)]
        public string Address
        {
            get
            {
                return Converter.ToString2(this["Address"]);
            }

            set
            {
                this["Address"] = value;
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
                this["IsActive"] = value;
            }
        }
    }
}
