// --------------------------------------------------------------------------
// <copyright file="IWinLossSummaryRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NAP.Framework.Data;

    public interface IWinLossSummaryRepository
    {
        List<dynamic> GetReport(ICriteria criteria, ref Hashtable outputParam);
    }
}
