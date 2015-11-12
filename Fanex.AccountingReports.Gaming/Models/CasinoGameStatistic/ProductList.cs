// -----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class ProductList
    {
        public ProductList()
        {
            ProductGroup = new List<string>();
            Products = new List<ProductItem>();
        }

        public List<string> ProductGroup
        {
            get;
            set;
        }

        public List<ProductItem> Products
        {
            get;
            set;
        }
    }
}
