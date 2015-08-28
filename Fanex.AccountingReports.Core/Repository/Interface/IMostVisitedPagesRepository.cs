// --------------------------------------------------------------------------
// <copyright file="IMostVisitedPagesRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.NAP.Framework.Data;

    public interface IMostVisitedPagesRepository
    {
        IEnumerable<MostVisitedPage> FetchMostVisitedPages(SelectMostVisitedPageCriteria criteria);

        void UpdateCount(ICriteria mostVisitedPage);

        void UpdatePosition(ICriteria mostVisitedPage);

        void Delete(ICriteria mostVisitedPage);
    }
}
