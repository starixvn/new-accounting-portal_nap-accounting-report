// --------------------------------------------------------------------------
// <copyright file="ITotalSMAMemberActiveService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive;
    using Fanex.Data.Repository;

    public interface ITotalSMAMemberActiveService
    {
        void ExportReport(GetTotalSMAMemberActiveCriteria criteria);

        void ExportStatusDetails(GetStatusDetailsCriteria criteria);

        IList<SMASummaryInfo> GetReport(GetTotalSMAMemberActiveCriteria criteria);

        IList<EntityBase<int, string>> GetSites(CriteriaBase criteria);

        IList<GroupStatusDetailInfo> GetStatusDetails(GetStatusDetailsCriteria criteria);
    }
}