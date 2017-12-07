namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SMAsTurnoverCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public SMAsTurnoverElement this[int index]
        {
            get
            {
                return (SMAsTurnoverElement)BaseGet(index);
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

        public new SMAsTurnoverElement this[string alias]
        {
            get
            {
                return (SMAsTurnoverElement)BaseGet(alias);
            }
        }

        public void Add(SMAsTurnoverElement element)
        {
            BaseAdd(element);
        }

        public void Remove(SMAsTurnoverElement element)
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

        public int IndexOf(SMAsTurnoverElement element)
        {
            return BaseIndexOf(element);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new SMAsTurnoverElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SMAsTurnoverElement)element).Id;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}
