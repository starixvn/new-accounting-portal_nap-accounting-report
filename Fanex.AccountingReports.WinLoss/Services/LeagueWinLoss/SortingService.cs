using Fanex.AccountingReports.Core.Constants;
using Fanex.AccountingReports.Core.Extensions;
using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.WinLoss.Services.LeagueWinLoss
{
    public class SortingService
    {
        public LeagueWinLossModel SortData(LeagueWinLossModel reportModel, string sortDirection, string sortColumnName)
        {
            SortStatisticsForPerChildGroup(reportModel, sortDirection, sortColumnName);
            SortStatisticsForParentGroup(reportModel, sortDirection, sortColumnName);
            return reportModel;
        }

        private static void SortStatisticsForPerChildGroup(LeagueWinLossModel reportModel, string sortDirection, string sortColumnName)
        {
            foreach (var row in reportModel.SportWinLoss)
            {
                ListExtensions.SortDataByProperty(row.Leagues, sortDirection, sortColumnName, "number");
            }
        }

        private static void SortStatisticsForParentGroup(LeagueWinLossModel reportModel, string sortDirection, string sortColumnName)
        {
            var propInfo = typeof(LeagueWinLossItem).GetProperty(sortColumnName);
            var isAsc = string.Equals(sortDirection, SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
            reportModel.SportWinLoss = isAsc ?
            reportModel
                .SportWinLoss
                .OrderBy(s => propInfo.GetValue(s.Total, null))
                .ToList() :
            reportModel
                 .SportWinLoss
                .OrderByDescending(s => propInfo.GetValue(s.Total, null))
                .ToList();
        }
    }
}
