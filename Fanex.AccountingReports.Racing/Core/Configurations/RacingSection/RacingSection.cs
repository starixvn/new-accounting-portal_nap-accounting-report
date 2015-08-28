// -----------------------------------------------------------------------
// <copyright file="RacingSection.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// The Racing Section.
    /// </summary>
    public class RacingSection : ConfigurationSection
    {
        public static RacingSection Instance
        {
            get
            {
                var storeSection =
                    ConfigurationManager.GetSection("RacingSection") as RacingSection;
                return storeSection;
            }
        }

        [ConfigurationProperty("ptSites", DefaultValue = "12bet", IsRequired = false)]
        public string PtSites
        {
            get { return (string)this["ptSites"]; }
            set { this["ptSites"] = value.ToLower(); }
        }
    }
}
