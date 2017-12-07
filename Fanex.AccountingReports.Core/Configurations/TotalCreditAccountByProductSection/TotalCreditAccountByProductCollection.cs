namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalCreditAccountByProductCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public TotalCreditAccountByProductElement this[int index]
        {
            get
            {
                return (TotalCreditAccountByProductElement)BaseGet(index);
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

        public new TotalCreditAccountByProductElement this[string alias]
        {
            get
            {
                return (TotalCreditAccountByProductElement)BaseGet(alias);
            }
        }

        public void Add(TotalCreditAccountByProductElement element)
        {
            BaseAdd(element);
        }

        public void Remove(TotalCreditAccountByProductElement element)
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

        public int IndexOf(TotalCreditAccountByProductElement element)
        {
            return BaseIndexOf(element);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new TotalCreditAccountByProductElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TotalCreditAccountByProductElement)element).Id;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
    }
}
