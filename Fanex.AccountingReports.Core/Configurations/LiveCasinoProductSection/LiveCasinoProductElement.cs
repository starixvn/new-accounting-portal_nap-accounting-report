// -----------------------------------------------------------------------
// <copyright file="LiveCasinoProductElement.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoProductElement : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get
            {
                return (string)this["Id"];
            }

            set
            {
                this["Id"] = value;
            }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
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

        [ConfigurationProperty("IsActive", IsRequired = true)]
        public bool IsActive
        {
            get
            {
                return (bool)this["IsActive"];
            }

            set
            {
                this["IsActive"] = value;
            }
        }
    }
}