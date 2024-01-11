namespace Fanex.AccountingReports.Core.Models
{
    using System;

    [Serializable]
    public class EntityBase<TId, TName> : IEntity<TId, TName>
    {
        public TId Id
        {
            get;
            set;
        }

        public TName Name
        {
            get;
            set;
        }
    }
}
