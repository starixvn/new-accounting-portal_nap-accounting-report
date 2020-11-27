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
            return reportModel;
        }

        private static void SortStatisticsForPerChildGroup(LeagueWinLossModel reportModel, string sortDirection, string sortColumnName)
        {
            foreach (var row in reportModel.SportWinLoss)
            {
                ListExtensions.SortDataByProperty(row.Leagues, sortDirection, sortColumnName, "number");
            }
        }

        public SiteWinLossModel SortSiteData(SiteWinLossModel reportModel, string sortDirection, string sortColumnName)
        {
            ListExtensions.SortDataByProperty(reportModel.Sites, sortDirection, sortColumnName, "number");
            return reportModel;
        }
    }
}