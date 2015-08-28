// --------------------------------------------------------------------------
// <copyright file="IBingoStatisticRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.BingoStatistic;

    public interface IBingoStatisticRepository
    {
        List<dynamic> GetReport(GetReportCriteria criteria);
    }
}
