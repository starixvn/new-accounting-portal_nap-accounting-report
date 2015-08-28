// --------------------------------------------------------------------------
// <copyright file="ExternalDailyWinlostViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Test.Models
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ExternalDailyWinlostViewModel
    {
        private double _totalTo;

        private double _comTo;

        private double _superTo;

        private double _masterTo;

        private double _agentTo;

        private double _companyWinlost;

        private double _superWinlost;

        private double _masterWinlost;

        private double _agentWinlost;

        private List<ExternalDailyWinlostDetail> _winlossDetails;

        public ExternalDailyWinlostViewModel()
        {
            _winlossDetails = new List<ExternalDailyWinlostDetail>();
        }

        #region Properties

        public string TotalTo
        {
            get { return _totalTo.Format(2); }
        }
        
        public string CompanyTo
        {
            get { return _comTo.Format(2); }
        }
        
        public string SuperTo
        {
            get { return _superTo.Format(2); }
        }

        public string MasterTo
        {
            get { return _masterTo.Format(2); }
        }

        public string AgentTo
        {
            get { return _agentTo.Format(2); }
        }

        public string CompanyWinlost
        {
            get { return _companyWinlost.Format(2); }
        }

        public string SuperWinlost
        {
            get { return _superWinlost.Format(2); }
        }

        public string MasterWinlost
        {
            get { return _masterWinlost.Format(2); }
        }

        public string AgentWinlost
        {
            get { return _agentWinlost.Format(2); }
        }

        public List<ExternalDailyWinlostDetail> WinlossDetails
        {
            get { return _winlossDetails; }
            set { _winlossDetails = value; }
        }
        #endregion
        
        #region PT

        public string CompanyPt
        {
            get
            {
                var comPt = _comTo / _totalTo * 100;
                return comPt.Format(2);
            }
        }

        public string SuperPt
        {
            get
            {
                var comPt = _superTo / _totalTo * 100;
                return comPt.Format(2);
            }
        }

        public string MasterPt
        {
            get
            {
                var comPt = _masterTo / _totalTo * 100;
                return comPt.Format(2);
            }
        }

        public string AgentPt
        {
            get
            {
                var comPt = _agentTo / _totalTo * 100;
                return comPt.Format(2);
            }
        }

        #endregion

        #region Margin

        public string CompanyMargin
        {
            get
            {
                var margin = _companyWinlost / _comTo * 100;
                return margin.Format(2);
            }
        }

        public string SuperMargin
        {
            get
            {
                var margin = _superWinlost / _superTo * 100;
                return margin.Format(2);
            }
        }

        public string MasterMargin
        {
            get
            {
                var margin = _masterWinlost / _masterTo * 100;
                return margin.Format(2);
            }
        }

        public string AgentMargin
        {
            get
            {
                var margin = _agentWinlost / _agentTo * 100;
                return margin.Format(2);
            }
        }

        #endregion

        /// <summary>
        /// Adds to list.
        /// </summary>
        /// <param name="dailyWinLossDetail">The daily win loss detail.</param>
        public void AddToList(ExternalDailyWinlostDetail dailyWinLossDetail)
        {
            if (dailyWinLossDetail == null)
            {
                return;
            }

            _totalTo += Convert.ToDouble(dailyWinLossDetail.TotalTo);
            _comTo += Convert.ToDouble(dailyWinLossDetail.CompanyTo);
            _superTo += Convert.ToDouble(dailyWinLossDetail.SuperTo);
            _masterTo += Convert.ToDouble(dailyWinLossDetail.MasterTo);
            _agentTo += Convert.ToDouble(dailyWinLossDetail.AgentTo);

            _companyWinlost += Convert.ToDouble(dailyWinLossDetail.CompanyWinlost);
            _superWinlost += Convert.ToDouble(dailyWinLossDetail.SuperWinlost);
            _masterWinlost += Convert.ToDouble(dailyWinLossDetail.MasterWinlost);
            _agentWinlost += Convert.ToDouble(dailyWinLossDetail.AgentWinlost);

            _winlossDetails.Add(dailyWinLossDetail);
        }
    }
}
