namespace Fanex.AccountingReports.Licensee.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailCollection : ConfigurationElementCollection
    {
        public MailCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public MailElement this[int index]
        {
            get
            {
                return (MailElement)BaseGet(index);
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

        public new MailElement this[string name]
        {
            get
            {
                return (MailElement)BaseGet(name);
            }
        }

        public void Add(MailElement mailElement)
        {
            BaseAdd(mailElement);
        }

        public void Remove(MailElement mailElement)
        {
            if (BaseIndexOf(mailElement) >= 0)
            {
                BaseRemove(mailElement.Name);
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

        public int IndexOf(MailElement mailElement)
        {
            return BaseIndexOf(mailElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override sealed ConfigurationElement CreateNewElement()
        {
            return new MailElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MailElement)element).Name;
        }
    }
}
