namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoProductService : ILiveCasinoProductService
    {
        public List<BaseModel> GetProductList()
        {
            List<BaseModel> result = new List<BaseModel>();
            LiveCasinoProductCollection collection = LiveCasinoProductSection.Instance.Products;
            foreach (LiveCasinoProductElement element in collection)
            {
                if (!element.IsActive)
                {
                    continue;
                }

                int productId = Converter.ToInt(element.Id);
                string productName = Converter.ToString2(element.Name);

                result.Add(new BaseModel()
                {
                    Id = productId,
                    Name = productName
                });
            }

            return result;
        }

        public List<BaseModel> GetU3ProductList()
        {
            List<BaseModel> result = new List<BaseModel>();
            LiveCasinoProductCollection collection = LiveCasinoProductSection.Instance.U3Products;
            foreach (LiveCasinoProductElement element in collection)
            {
                if (!element.IsActive)
                {
                    continue;
                }

                int productId = Converter.ToInt(element.Id);
                string productName = Converter.ToString2(element.Name);

                result.Add(new BaseModel()
                {
                    Id = productId,
                    Name = productName
                });
            }

            return result;
        }
    }
}
