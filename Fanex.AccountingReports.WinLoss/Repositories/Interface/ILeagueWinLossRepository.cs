// --------------------------------------------------------------------------
// <copyright file="ILeagueWinLossRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;

    public interface ILeagueWinLossRepository
    {
        List<dynamic> GetLeagueWinLoss(ICriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetLeagueWinLossBySport(
            ICriteria criteria,
            out List<dynamic> eightRMTable, 
            out List<dynamic> barmTable);

        List<dynamic> GetLeagueWinLossByBetType(ICriteria criteria, ref Hashtable outputParam);
    }
}
