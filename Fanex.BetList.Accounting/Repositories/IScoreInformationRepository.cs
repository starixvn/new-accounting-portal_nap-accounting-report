// --------------------------------------------------------------------------
// <copyright file="IScoreInformationRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>
namespace Fanex.BetList.Accounting.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.Result.Core.Sportsbook.DTO;
    
    public interface IScoreInformationRepository
    {
        List<Sportsbook> GetScoreInformation(GetScoreInformationCriteria criteria, ref Hashtable outputParams);

        List<dynamic> GetSportingSolutionsResult(GetSportingSolutionsResultCriteria criteria);
    }
}