namespace Fanex.AccountingReports.Core.Models
{
    using System.Collections.Generic;

    public class Report<TDetail> : EntityBase<int, string>
    {
        public IList<Report<TDetail>> Childs { get; set; }

        public TDetail Details { get; set; }
    }
}
