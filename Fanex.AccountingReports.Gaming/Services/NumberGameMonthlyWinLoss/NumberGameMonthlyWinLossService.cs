namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameMonthlyWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;

    public class NumberGameMonthlyWinLossService : ServiceBase
    {
        private string _numberGameMonthlyWinLossReportSessionKey = "Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLoss." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private NumberGameMonthlyWinLossRepository _numberGameMonthlyWinLossRepository;

        public NumberGameMonthlyWinLossService()
        {
            this._numberGameMonthlyWinLossRepository = new NumberGameMonthlyWinLossRepository();
        }

        public NumberGameMonthlyWinLoss GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var result = new NumberGameMonthlyWinLoss();

            var rawData = _numberGameMonthlyWinLossRepository.GetNumberGameMonthlyWinLoss(getNumberGameMonthlyWinLossCriteria);

            if (rawData == null)
            {
                return result;
            }

            result.ReportRows = rawData;

            foreach (var row in result.ReportRows)
            {
                result.CalculateTotal(row);
            }

            SessionManager.Set<NumberGameMonthlyWinLoss>(_numberGameMonthlyWinLossReportSessionKey, result);
            return result;
        }
    }
}
