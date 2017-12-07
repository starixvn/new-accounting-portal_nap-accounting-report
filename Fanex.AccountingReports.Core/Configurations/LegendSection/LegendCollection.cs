namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LegendCollection : ConfigurationElementCollection
    {
        public LegendCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public LegendElement this[int index]
        {
            get
            {
                return (LegendElement)BaseGet(index);
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

        public new LegendElement this[string alias]
        {
            get
            {
                return (LegendElement)BaseGet(alias);
            }
        }

        public void Add(LegendElement element)
        {
            BaseAdd(element);
        }

        public void Remove(LegendElement element)
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

        public void Remove(string alias)
        {
            BaseRemove(alias);
        }

        public void Clear()
        {
            BaseClear();
        }

        public int IndexOf(LegendElement element)
        {
            return BaseIndexOf(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new LegendElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LegendElement)element).Id;
        }
    }
}
