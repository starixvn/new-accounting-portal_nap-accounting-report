namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoProductCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public LiveCasinoProductElement this[int index]
        {
            get
            {
                return (LiveCasinoProductElement)BaseGet(index);
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

        public new LiveCasinoProductElement this[string name]
        {
            get
            {
                return (LiveCasinoProductElement)BaseGet(name);
            }
        }

        public void Add(LiveCasinoProductElement element)
        {
            BaseAdd(element);
        }

        public void Remove(LiveCasinoProductElement element)
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

        public int IndexOf(LiveCasinoProductElement element)
        {
            return BaseIndexOf(element);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new LiveCasinoProductElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LiveCasinoProductElement)element).Id;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}
