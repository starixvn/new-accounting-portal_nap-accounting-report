namespace Fanex.AccountingReports.Test.Models
{
    using System;
    using Fanex.AccountingReports.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ExternalDailyWinlostDetail
    {
        private string _totalTo;

        private string _comTo;

        private string _superTo;

        private string _masterTo;

        private string _agentTo;

        private string _companyWinlost;

        private string _superWinlost;

        private string _masterWinlost;

        private string _agentWinlost;

        public int CustId { get; set; }

        public string Username { get; set; }

        public string TotalTo
        {
            get { return _totalTo; }
            set { _totalTo = Convert.ToDouble(value).Format(2); }
        }

        public string CompanyTo
        {
            get { return _comTo; }
            set { _comTo = Convert.ToDouble(value).Format(2); }
        }

        public string SuperTo
        {
            get { return _superTo; }
            set { _superTo = Convert.ToDouble(value).Format(2); }
        }

        public string MasterTo
        {
            get { return _masterTo; }
            set { _masterTo = Convert.ToDouble(value).Format(2); }
        }

        public string AgentTo
        {
            get { return _agentTo; }
            set { _agentTo = Convert.ToDouble(value).Format(2); }
        }

        public string CompanyWinlost
        {
            get { return _companyWinlost; }
            set { _companyWinlost = Convert.ToDouble(value).Format(2); }
        }

        public string SuperWinlost
        {
            get { return _superWinlost; }
            set { _superWinlost = Convert.ToDouble(value).Format(2); }
        }

        public string MasterWinlost
        {
            get { return _masterWinlost; }
            set { _masterWinlost = Convert.ToDouble(value).Format(2); }
        }

        public string AgentWinlost
        {
            get { return _agentWinlost; }
            set { _agentWinlost = Convert.ToDouble(value).Format(2); }
        }

        #region PT

        public string CompanyPt
        {
            get
            {
                var comPt = Convert.ToDouble(CompanyTo) / Convert.ToDouble(TotalTo) * 100;
                return comPt.Format(2);
            }
        }

        public string SuperPt
        {
            get
            {
                var comPt = Convert.ToDouble(SuperTo) / Convert.ToDouble(TotalTo) * 100;
                return comPt.Format(2);
            }
        }

        public string MasterPt
        {
            get
            {
                var comPt = Convert.ToDouble(MasterTo) / Convert.ToDouble(TotalTo) * 100;
                return comPt.Format(2);
            }
        }

        public string AgentPt
        {
            get
            {
                var comPt = Convert.ToDouble(AgentTo) / Convert.ToDouble(TotalTo) * 100;
                return comPt.Format(2);
            }
        }

        #endregion

        #region Margin

        public string CompanyMargin
        {
            get
            {
                var margin = Convert.ToDouble(CompanyWinlost) / Convert.ToDouble(CompanyTo) * 100;
                return margin.Format(2);
            }
        }

        public string SuperMargin
        {
            get
            {
                var margin = Convert.ToDouble(SuperWinlost) / Convert.ToDouble(SuperTo) * 100;
                return margin.Format(2);
            }
        }

        public string MasterMargin
        {
            get
            {
                var margin = Convert.ToDouble(MasterWinlost) / Convert.ToDouble(MasterTo) * 100;
                return margin.Format(2);
            }
        }

        public string AgentMargin
        {
            get
            {
                var margin = Convert.ToDouble(AgentWinlost) / Convert.ToDouble(AgentTo) * 100;
                return margin.Format(2);
            }
        }

        #endregion
    }
}
