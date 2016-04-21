// --------------------------------------------------------------------------
// <copyright file="IMatchWinLoss2aRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.Data.Repository;

    public interface IMatchWinLoss2aRepository
    {
        List<dynamic> GetMainReport(ICriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetWinLossByBetType(GetWinLossByBetTypeCriteria criteria);

        List<dynamic> GetWinLossBySportType(GetMainReportCriteria criteria, ref Hashtable outputParam);

        WinLossInfo GetWinLossDetailBeforeBalanceUpDay(GetDetailReportByBetTypeCriteria criteria);

        List<dynamic> GetAllDetailReport(GetMainReportCriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetAllDetailReportBySportsBeforeBalanceUpDay(GetAllDetailReportBySportsCriteria criteria, ref Hashtable outputParam);

        WinLossInfo GetThirdPartyCasinoReport(GetThirdPartyCasinoReportCriteria criteria);

        WinLossInfo GetMetricGamingDetailByBetType(GetMetricGamingDetailByBetTypeCriteria criteria);
    }
}