namespace Fanex.AccountingReports.Balance.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Balance.Criteria.SuperBalance;
    using Fanex.AccountingReports.Balance.Models.SuperBalance;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ISuperBalanceService
    {
        SuperBalance GetSuperBalance(GetSuperBalanceCriteria superBalanceCriteria);

        SuperBalance GetSuperBalanceSuperDownLine(GetSuperBalanceSuperDownlineCriteria superBalanceSuperDownlineCriteria);

        SuperBalance GetSuperBalanceMasterDownLine(GetSuperBalanceMasterDownlineCriteria superBalanceMasterDownlineCriteria);

        SuperBalance GetSuperBalanceAgentDownLine(GetSuperBalanceAgentDownlineCriteria superBalanceAgentDownlineCriteria);

        SuperBalance ExportSuperBalanceReport(GetSuperBalanceCriteria superBalanceCriteria);

        SuperBalance ExportBalanceSuperDownLineReport(GetSuperBalanceSuperDownlineCriteria getSuperBalanceSuperCriteria);

        SuperBalance ExportBalanceMasterDownLineReport(GetSuperBalanceMasterDownlineCriteria getSuperBalanceMasterCriteria);

        SuperBalance ExportBalanceAgentDownLineReport(GetSuperBalanceAgentDownlineCriteria getSuperBalanceAgentCriteria);

        List<EntityBase<int, string>> GetCurrencies();

        List<string> GetListAlphabet();
    }
}
