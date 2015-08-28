// -----------------------------------------------------------------------
// <copyright file="VirtualSportsElement.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Core.Configurations.VirtualSportsWinLossSection
{
    using System.Configuration;

    /// <summary>
    /// The element of Virtual Sports collection.
    /// </summary>
    public class VirtualSportsElement : ConfigurationElement
    {
        [ConfigurationProperty("id", DefaultValue = "0", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("name", DefaultValue = "NA", IsRequired = true, IsKey = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("active", DefaultValue = true, IsRequired = true, IsKey = false)]
        public bool Active
        {
            get { return (bool)this["active"]; }
            set { this["active"] = value; }
        }
    }
}
