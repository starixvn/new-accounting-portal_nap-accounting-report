// --------------------------------------------------------------------------
// <copyright file="TransactionItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.TotalTransactions
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class TransactionItem : EntityBase<int, string>
    {
        public int BetCount { get; set; }

        public decimal EURCost { get; set; }

        /// <summary>
        /// Gets EURCostString: use for displaying.
        /// </summary>
        /// <value>
        /// The EUR cost string.
        /// </value>
        public string EURCostString
        {
            get
            {
                return ((Name.ToLower() == Resellers.VCAsia.ToString().ToLower() &&
                    (this.Parent != null && this.Parent.Name.ToLower() == Resellers.VCAsia.ToString().ToLower()))
                    || Id == (int)Resellers.VCAsia)
                    ? Labels.NotAvailable : this.EURCost.Format(2);
            }
        }

        public List<TransactionItem> Childs { get; set; }

        public TransactionItem Parent { get; set; }
    }
}