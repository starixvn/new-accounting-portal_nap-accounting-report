using Fanex.AccountingReports.Core.Models;
using System;
using System.Collections.Generic;

namespace Fanex.AccountingReports.Misc.Models.BetCount
{
    [Serializable]
    public class ProductInfo : EntityBase<int, string>
    {
        public List<ProductInfo> ChildsInfo { get; set; }
    }
}
