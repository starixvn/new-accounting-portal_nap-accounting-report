// -----------------------------------------------------------------------
// <copyright file="TotalCreditAccountByProduct.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models.TotalCreditAccountByProduct
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Total credit account by product.
    /// </summary>
    [Serializable]
    public class TotalCreditAccountByProduct
    {
        public TotalCreditAccountByProduct()
        {
            ReportRows = new List<TotalCreditAccountByProduct>();
        }

        public string UserName { get; set; }

        public int CustId { get; set; }

        public bool IsDisable { get; set; }

        public string Currency { get; set; }

        public int MasterEnable { get; set; }

        public int MasterDisable { get; set; }

        public int AgentEnable { get; set; }

        public int AgentDisable { get; set; }

        public int MemberEnable { get; set; }

        public int MemberDisable { get; set; }

        public List<TotalCreditAccountByProduct> ReportRows { get; set; }

        public TotalCreditAccountByProduct Total { get; set; }

        public int ReportLength { get; set; }

        public void CalculateTotal(TotalCreditAccountByProduct totalCreditAccountByProduct)
        {
            this.MasterEnable += totalCreditAccountByProduct.MasterEnable;
            this.MasterDisable += totalCreditAccountByProduct.MasterDisable;
            this.AgentEnable += totalCreditAccountByProduct.AgentEnable;
            this.AgentDisable += totalCreditAccountByProduct.AgentDisable;
            this.MemberEnable += totalCreditAccountByProduct.MemberEnable;
            this.MemberDisable += totalCreditAccountByProduct.MemberDisable;
        }
    }
}