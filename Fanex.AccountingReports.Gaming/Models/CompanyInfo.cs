// --------------------------------------------------------------------------
// <copyright file="CompanyInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class CompanyInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string EffectiveDate { get; set; }

        public List<CompanyInfo> ChildInfo { get; set; }
    }
}
