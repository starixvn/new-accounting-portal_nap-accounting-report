// --------------------------------------------------------------------------
// <copyright file="Reseller.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class Reseller : EntityBase<int, string>
    {
        public Reseller()
        {
            this.Licensees = new List<Licensee>();
        }

        public List<Licensee> Licensees { get; set; }

        public override bool Equals(object obj)
        {
            return this.Id == (obj as Reseller).Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() + Name.GetHashCode();
        }
    }
}