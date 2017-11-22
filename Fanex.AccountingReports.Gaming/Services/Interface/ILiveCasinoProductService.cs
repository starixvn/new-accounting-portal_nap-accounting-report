namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILiveCasinoProductService
    {
        List<BaseModel> GetProductList();

        List<BaseModel> GetU3ProductList();
    }
}
