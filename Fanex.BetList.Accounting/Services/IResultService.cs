// --------------------------------------------------------------------------
// <copyright file="IResultService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David>Tran" date="2014.10.24">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Models;
    using Fanex.NAP.Framework.Data;

    public interface IResultService
    {
        List<TennisLeagueResult> GetTennisResult(CriteriaBase criteria, string matchId, int bettypeId);
    }
}