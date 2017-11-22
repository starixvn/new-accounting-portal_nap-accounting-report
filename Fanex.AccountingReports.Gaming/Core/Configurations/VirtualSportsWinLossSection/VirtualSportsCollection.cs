namespace Fanex.AccountingReports.Gaming.Core.Configurations.VirtualSportsWinLossSection
{
    using System.Configuration;

    /// <summary>
    /// Virtual Sports collection.
    /// </summary>
    public class VirtualSportsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public VirtualSportsElement this[int index]
        {
            get
            {
                return (VirtualSportsElement)BaseGet(index);
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

        public new VirtualSportsElement this[string name]
        {
            get 
            {
                return (VirtualSportsElement)BaseGet(name);
            }
        }

        public void Add(VirtualSportsElement virtualSportsElement)
        {
            BaseAdd(virtualSportsElement);
        }

        public void Remove(VirtualSportsElement virtualSportsElement)
        {
            if (BaseIndexOf(virtualSportsElement) >= 0)
            {
                BaseRemove(virtualSportsElement);
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

        public int IndexOf(VirtualSportsElement virtualSportsElement)
        {
            return BaseIndexOf(virtualSportsElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            base.BaseAdd(element, false);
        }

        protected sealed override ConfigurationElement CreateNewElement()
        {
            return new VirtualSportsElement();
        }

        protected sealed override object GetElementKey(ConfigurationElement element)
        {
            return ((VirtualSportsElement)element).Id;
        }
    }
}
