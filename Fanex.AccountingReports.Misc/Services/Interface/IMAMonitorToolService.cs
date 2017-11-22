namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Misc.Criteria.MAMonitorTool;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;

    public interface IMAMonitorToolService
    {
        List<EntityBase<int, string>> GetReportTypeList();

        List<BaseModel> GetProductList();

        List<EntityBase<int, string>> GetSportList();

        List<EntityBase<int, string>> GetBetTypeList(GetBetTypeListCriteria criteria);

        List<ProductBettype> GetProductBettype(GetBetTypeListCriteria criteria);

        List<EntityBase<int, string>> GetCurrencyList();

        List<BaseModel> GetLeagueTypeList();

        List<EntityBase<int, string>> GetLeagueList(GetLeagueListCriteria criteria);

        WinLossInfo GetReport(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);
    }
}
