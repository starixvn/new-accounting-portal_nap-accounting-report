// -----------------------------------------------------------------------
// <copyright file="NumberGameStatisticByUserName.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.NumberGameStatistic
{
    using System.Collections.Generic;

    public class NumberGameStatisticByUserName
    {
        public NumberGameStatisticByUserName()
        {
            ReportRows = new List<NumberGameStatisticByUserNameDetail>();
        }

        public List<NumberGameStatisticByUserNameDetail> ReportRows { get; set; }

        public NumberGameStatisticByUserNameDetail GrandTotal { get; set; }
    }
}