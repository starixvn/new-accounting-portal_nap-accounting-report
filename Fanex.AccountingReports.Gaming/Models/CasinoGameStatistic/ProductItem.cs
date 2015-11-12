// -----------------------------------------------------------------------
// <copyright file="ProductItem.cs" company="Microsoft">
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
    public class ProductItem
    {
        public int ProductId 
        { 
            get;
            set; 
        }

        public string ProductName
        {
            get;
            set;
        }

        public string ProductGroup
        {
            get;
            set;
        }
    }
}
