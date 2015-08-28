// --------------------------------------------------------------------------
// <copyright file="ProductInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.PunterAnalysis
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ProductInfo : EntityBase<int, string>
    {
        public List<ProductInfo> ChildsInfo { get; set; }
    }
}
