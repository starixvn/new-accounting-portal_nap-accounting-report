namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.Common;

    public class InstallmentAmountService : ServiceBase
    {
        private readonly CreditTermRepository _creditTermRepository;

        private string _installmentAmountSessionKey = string.Empty;

        public InstallmentAmountService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._installmentAmountSessionKey = CreditTermExcelUtil.InstallmentAmountPrefixSessionKey
                                                    + AppSession.CurrentUser.Username;
            }
            else
            {
                this._installmentAmountSessionKey = CreditTermExcelUtil.InstallmentAmountPrefixSessionKey;
            }

            this._creditTermRepository = new CreditTermRepository();
        }

        public List<InstallAmount> GetInstallmentAmountReport(GetInstallAmountCriteria criteria)
        {
            var report = new List<InstallAmount>();

            if (!criteria.IsValid())
            {
                return report;
            }

            report = this.ConvertDataReport(criteria, report);

            return report;
        }

        private List<InstallAmount> ConvertDataReport(
            GetInstallAmountCriteria criteria,
            List<InstallAmount> report)
        {
            var rowsData = GetReportData<dynamic>(criteria);

            GetCurrencyByLicenseeCriteria currencyCriteria = new GetCurrencyByLicenseeCriteria();
            var currencies = ReportService.GetCurrencyByLicensee(currencyCriteria);

            foreach (var row in rowsData)
            {
                var currencyID = Converter.ToInt(row.curr);
                var currencyName = currencies.Where(s => s.Id == currencyID)
                                             .Select(s => s.Name).FirstOrDefault();

                var installAmountRow = new InstallAmount
                {
                    Username = Converter.ToString2(row.username),
                    CurrencyName = currencyName,
                    Balance = Convert.ToDouble(row.balance),
                    BalanceBase = Convert.ToDouble(row.balance_base),
                    Reward = Convert.ToDouble(row.Reward),
                    RewardRM = Convert.ToDouble(row.RewardRM),
                    InstallmentAmount = Convert.ToDouble(row.installmentamount),
                    NumberDayInstallment = Converter.ToInt(row.installmentdate)
                };

                report.Add(installAmountRow);
            }

            return report;
        }
    }
}
