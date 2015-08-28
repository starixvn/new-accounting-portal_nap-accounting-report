// --------------------------------------------------------------------------
// <copyright file="GetTotalTransactionsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.TotalTransactions
{
    using System;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTotalTransactionsCriteria : CriteriaBase
    {
        private string _normalLicensee;
        private string _betAggregatorLicensee;

        public GetTotalTransactionsCriteria()
        {
            this.FromDate = DateTime.Now.FirstDayOfMonth();
        }

        public GetTotalTransactionsCriteria(DateTime requestedDate)
            : this()
        {
            this.FromDate = requestedDate.FirstDayOfMonth();
            this.ToDate = requestedDate.LastDayOfMonth();
        }

        public string NormalLicensee
        {
            get
            {
                return string.IsNullOrWhiteSpace(_normalLicensee) ? string.Empty : _normalLicensee;
            }

            set
            {
                _normalLicensee = value;
            }
        }

        public string BALicensee
        {
            get
            {
                return string.IsNullOrWhiteSpace(_betAggregatorLicensee) ? string.Empty : _betAggregatorLicensee;
            }

            set
            {
                _betAggregatorLicensee = value;
            }
        }

        [SpParam(Ignored = true)]
        public string NormalLicenseeName { get; set; }

        [SpParam(Ignored = true)]
        public string BALicenseeName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool ShowSB { get; set; }

        public bool ShowRB { get; set; }

        public bool ShowNG { get; set; }

        public bool ShowLC { get; set; }

        public bool ShowVS { get; set; }

        public bool ShowCS { get; set; }

        public bool ShowKN { get; set; }

        public bool ShowGD { get; set; }

        public bool ShowAGCS { get; set; }

        public bool ShowVG { get; set; }

        public int GroupBy
        {
            get
            {
                return 2; // group by monthly
            }
        }

        public override string GetSettingKey()
        {
            return "Licensee_TotalTransaction_GetTotalTicket";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
