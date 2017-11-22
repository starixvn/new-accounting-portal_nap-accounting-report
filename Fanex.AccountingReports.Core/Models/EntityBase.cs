namespace Fanex.AccountingReports.Core.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    /// <typeparam name="TName">The type of the name.</typeparam>
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
