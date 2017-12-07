namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Models.TotalCreditAccountByProduct;

    public interface ITotalCreditAccountByProductService
    {
        List<BaseModel> GetProducts();

        List<EntityBase<int, string>> GetCurrencies();

        TotalCreditAccountByProduct GetTotalCreditAccountByProduct(GetTotalCreditAccountByProductCriteria criteria);

        BetCountDetail GetBetCountDetail(GetBetCountDetailCriteria criteria);

        void ExportTotalCreditAccountByProductData(GetTotalCreditAccountByProductCriteria criteria);

        void ExportBetCountDetailData(GetBetCountDetailCriteria criteria);
    }
}
