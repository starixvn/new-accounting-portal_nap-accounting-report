// --------------------------------------------------------------------------
// <copyright file="IScoreInformationService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Models.ScoreInformation;

    public interface IScoreInformationService
    {
        ScoreInformation GetScoreInfo(GetScoreInformationCriteria criteria);
    }
}