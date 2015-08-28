// --------------------------------------------------------------------------
// <copyright file="ITotalSMAMemberActiveRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.15">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.NAP.Framework.Data;

    public interface ITotalSMAMemberActiveRepository
    {
        void GetReport(ICriteria criteria, ref dynamic totalSMA, ref IList<dynamic> activeDetails);

        IList<dynamic> GetStatusDetail(GetStatusDetailsCriteria criteria, ref Hashtable outputParam);
    }
}