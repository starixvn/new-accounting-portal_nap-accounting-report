// --------------------------------------------------------------------------
// <copyright file="WinLossMemberBetListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossMemberBetListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "CustID")]
        public int CustID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public bool ShowSB { get; set; }

        public bool ShowRB { get; set; }

        public bool ShowNG { get; set; }

        public bool ShowBA { get; set; }

        public bool ShowLC { get; set; }

        public bool ShowVS { get; set; }

        public bool ShowCS { get; set; }

        public bool ShowKN { get; set; }

        public bool ShowMGC { get; set; }

        /// <summary>
        /// Gets or sets the base currency:
        /// 0: ALL, 1: USD, 2: MYR, 3: EUR.
        /// </summary>
        /// <value>
        /// The base currency.
        /// </value>
        public int BaseCurrency { get; set; }

        public int TotalRows { get; set; }

        public double SumWinlost { get; set; }

        public double SumWinlostUSD { get; set; }

        public double SumWinlostEUR { get; set; }

        public double SumLicenseeWL { get; set; }

        public double SumLicenseeWLUSD { get; set; }

        public double SumLicenseeWLEUR { get; set; }

        public double SumComm { get; set; }

        public double SumCommUSD { get; set; }

        public double SumCommEUR { get; set; }

        public double SumPlayerComm { get; set; }

        public double SumPlayerCommUSD { get; set; }

        public double SumPlayerCommEUR { get; set; }

        public int Drawfilter { get; set; }

        public string Language { get; set; }

        public int CurrencyId { get; set; }

        [SpParam(Ignored = true)]
        public string CustName { get; set; }

        public override string GetSettingKey()
        {
            return "BetList_MemberWinLoss";
        }
    }
}