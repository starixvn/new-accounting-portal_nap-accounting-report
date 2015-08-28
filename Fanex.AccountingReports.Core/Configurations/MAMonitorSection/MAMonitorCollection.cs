// --------------------------------------------------------------------------
// <copyright file="MAMonitorCollection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MAMonitorCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public MAMonitorElement this[int index]
        {
            get
            {
                return (MAMonitorElement)BaseGet(index);
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

        public new MAMonitorElement this[string alias]
        {
            get
            {
                return (MAMonitorElement)BaseGet(alias);
            }
        }

        public void Add(MAMonitorElement element)
        {
            BaseAdd(element);
        }

        public void Remove(MAMonitorElement element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.Id);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string id)
        {
            BaseRemove(id);
        }

        public void Clear()
        {
            BaseClear();
        }

        public int IndexOf(MAMonitorElement element)
        {
            return BaseIndexOf(element);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new MAMonitorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MAMonitorElement)element).Id;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}
