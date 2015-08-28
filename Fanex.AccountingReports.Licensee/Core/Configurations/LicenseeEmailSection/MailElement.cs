// --------------------------------------------------------------------------
// <copyright file="MailElement.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Core.Configurations
{
    using System.Configuration;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailElement : ConfigurationElement
    {
        public MailElement()
        {
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }

            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("to", IsRequired = false)]
        public string To
        {
            get
            {
                return (string)this["to"];
            }

            set
            {
                this["to"] = value;
            }
        }

        [ConfigurationProperty("bcc", IsRequired = false)]
        public string Bcc
        {
            get
            {
                return (string)this["bcc"];
            }

            set
            {
                this["bcc"] = value;
            }
        }

        [ConfigurationProperty("cc", IsRequired = false)]
        public string Cc
        {
            get
            {
                return (string)this["cc"];
            }

            set
            {
                this["cc"] = value;
            }
        }
    }
}
