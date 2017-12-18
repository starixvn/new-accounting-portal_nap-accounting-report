namespace Fanex.AccountingReports.Special.Criteria.SMATurnover
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SMATurnoverCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public bool ShowSB { get; set; }

        public bool ShowRB { get; set; }

        public bool ShowNG { get; set; }

        public bool ShowKN { get; set; }

        public bool ShowLC { get; set; }

        public bool ShowVS { get; set; }

        public bool ShowCS { get; set; }

        public bool ShowOpened { get; set; }

        public bool ShowClosed { get; set; }

        public bool ShowSuspended { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        public override bool IsValid()
        {
            return this.MoveBetsDay < this.WinlostDate;
        }

        public override string GetSettingKey()
        {
            return "Special_SMAsTurnOver";
        }
    }
}