// -----------------------------------------------------------------------
// <copyright file="TotalCreditAccountByProductElement.cs" company="Nexcel Solutions Vietnam">
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
    public class TotalCreditAccountByProductElement : ConfigurationElement
    {
        public TotalCreditAccountByProductElement()
        {
        }

        [ConfigurationProperty("Id", IsRequired = true)]
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

        [ConfigurationProperty("AliasName", IsRequired = false)]
        public string AliasName
        {
            get
            {
                return (string)this["AliasName"];
            }

            set
            {
                this["AliasName"] = value;
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