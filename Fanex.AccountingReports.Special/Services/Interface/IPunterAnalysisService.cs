// --------------------------------------------------------------------------
// <copyright file="IPunterAnalysisService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Special.Criteria.PunterAnalysis;
    using Fanex.AccountingReports.Special.Models.PunterAnalysis;

    public interface IPunterAnalysisService
    {
        List<ProductInfo> GetProducts(GetProductsCriteria criteria);

        List<ProductInfo> GetSports(GetProductsCriteria criteria);

        List<EntityBase<int, string>> GetLeaguesBySport(GetLeaguesBySportCriteria criteria);

        List<EntityBase<int, string>> GetBetTypesBySport(GetBetTypesBySportCriteria criteria);

        PunterWinLossInfo GetTopPunterReport(GetTopPunterReportCriteria criteria);

        PunterWinLossInfo GetTopPunterByUser(GetTopPunterByUserCriteria criteria);

        PunterWinLossInfo GetTopPunterBetCount_ByUser(GetTopPunterBetCountByUserCriteria criteria);

        PunterWinLossInfo GetTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria);

        PunterWinLossInfo GetTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria);

        PunterWinLossInfo GetTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria);

        PunterWinLossInfo GetTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria);

        PunterWinLossInfo GetTopPunter_ByLeague(GetTopPunter_ByLeagueCriteria criteria);

        PunterWinLossInfo GetTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria);

        void ExportTopPunterReport(GetTopPunterReportCriteria criteria);

        void ExportTopPunterByUser(GetTopPunterByUserCriteria criteria);

        void ExportTopPunterBetCount_ByUser(GetTopPunterBetCountByUserCriteria criteria);

        void ExportTopPunterIP_ByUser(GetTopPunterIPByUserCriteria criteria);

        void ExportTopPunterBetCount_ByUser_BetType(GetTopPunterBetCount_ByUser_BetTypeCriteria criteria);

        void ExportTopPunter_ByUser_SportType(GetTopPunter_ByUser_SportTypeCriteria criteria);

        void ExportTopPunter_ByUser_BetType(GetTopPunter_ByUser_BetTypeCriteria criteria);

        void ExportTopPunter_ByLeague(GetTopPunter_ByLeagueCriteria criteria);

        void ExportTopPunter_ByLeague_User(GetTopPunter_ByLeague_UserCriteria criteria);
    }
}
