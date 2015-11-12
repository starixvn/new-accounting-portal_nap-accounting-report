// -----------------------------------------------------------------------
// <copyright file="IExportExcelHourlyService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IExportExcelHourlyService
    {
        void ExportReport(GetReportByHourlyCriteria criteria);
    }
}
