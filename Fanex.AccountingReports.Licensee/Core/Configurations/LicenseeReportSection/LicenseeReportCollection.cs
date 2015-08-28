// --------------------------------------------------------------------------
// <copyright file="LicenseeReportCollection.cs" company="Nexcel Solutions Vietnam">
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
    public class LicenseeReportCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public LicenseeReportElement this[int index]
        {
            get
            {
                return (LicenseeReportElement)BaseGet(index);
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public new LicenseeReportElement this[string name]
        {
            get
            {
                return (LicenseeReportElement)BaseGet(name);
            }
        }

        public void Add(LicenseeReportElement licenseeReportElement)
        {
            BaseAdd(licenseeReportElement);
        }

        public void Remove(LicenseeReportElement licenseeReportElement)
        {
            if (BaseIndexOf(licenseeReportElement) >= 0)
            {
                BaseRemove(licenseeReportElement.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string alias)
        {
            BaseRemove(alias);
        }

        public void Clear()
        {
            BaseClear();
        }

        public int IndexOf(LicenseeReportElement licenseeReportElement)
        {
            return BaseIndexOf(licenseeReportElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new LicenseeReportElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LicenseeReportElement)element).Name;
        }
    }
}