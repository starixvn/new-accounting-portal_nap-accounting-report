// --------------------------------------------------------------------------
// <copyright file="IDailyWinLoss2hSoccerRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer;

    public interface IDailyWinLoss2hSoccerRepository
    {
        List<dynamic> GetMain2hReport(GetMain2hReportCriteria criteria, ref Hashtable outputParam);
    }
}
