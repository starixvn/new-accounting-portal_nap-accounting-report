// --------------------------------------------------------------------------
// <copyright file="LicenseeReportElement.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeReportElement : ConfigurationElement
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

        [ConfigurationProperty("active", DefaultValue = true, IsRequired = true, IsKey = true)]
        public bool Active
        {
            get
            {
                return (bool)this["active"];
            }

            set
            {
                this["active"] = value;
            }
        }

        [ConfigurationProperty("resellerids", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string ResellerIds
        {
            get
            {
                var resellerids = (string)this["resellerids"];
                return string.IsNullOrWhiteSpace(resellerids) ? string.Empty : resellerids.Trim();
            }

            set
            {
                this["resellerids"] = value;
            }
        }

        [ConfigurationProperty("resellers", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Resellers
        {
            get
            {
                var resellers = (string)this["resellers"];
                return string.IsNullOrWhiteSpace(resellers) ? string.Empty : resellers.Trim();
            }

            set
            {
                this["resellers"] = value;
            }
        }
    }
}