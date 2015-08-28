// -----------------------------------------------------------------------
// <copyright file="IMarginWinLossService.cs" company="Nexcel Solutions Vietname">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Services.Interface.MarginReport
{
    using Fanex.AccountingReports.Special.Criteria.MarginReport;
    using Fanex.AccountingReports.Special.Models.MarginReport;

    /// <summary>
    /// Interface MarginWinLossService.
    /// </summary>
    public interface IMarginWinLossService
    {
        MarginWinLoss GetDataMarginReport(GetMarginWinLossCriteria criteria);

       void ExcelReport(GetMarginWinLossCriteria criteria);
    }
}
