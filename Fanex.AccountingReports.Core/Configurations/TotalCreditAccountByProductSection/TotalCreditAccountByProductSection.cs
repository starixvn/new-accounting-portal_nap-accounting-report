// -----------------------------------------------------------------------
// <copyright file="TotalCreditAccountByProductSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.05">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalCreditAccountByProductSection : ConfigurationSection
    {
        public static TotalCreditAccountByProductSection Instance
        {
            get
            {
                var smaTurnover = ConfigurationManager.GetSection("TotalCreditAccountByProductSection") as TotalCreditAccountByProductSection;
                return smaTurnover;
            }
        }

        [ConfigurationProperty("Products", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TotalCreditAccountByProductCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public TotalCreditAccountByProductCollection Products
        {
            get
            {
                var products = (TotalCreditAccountByProductCollection)base["Products"];
                return products;
            }
        }
    }
}