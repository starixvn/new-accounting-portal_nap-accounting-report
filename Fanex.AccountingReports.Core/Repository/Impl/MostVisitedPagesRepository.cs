// --------------------------------------------------------------------------
// <copyright file="MostVisitedPagesRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Repository.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Repository.Interface;
    using Fanex.Common.Extensions;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MostVisitedPagesRepository : BaseRepository<MostVisitedPage>, IMostVisitedPagesRepository
    {
        /// <summary>
        /// Fetches the most visited pages.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The most visited pages.</returns>
        public IEnumerable<MostVisitedPage> FetchMostVisitedPages(SelectMostVisitedPageCriteria criteria)
        {
            // Create Function will map Data from DataReader to Schedule Object
            Func<IDataReader, MostVisitedPage> mapObjectFunc = (reader11) =>
            {
                return new MostVisitedPage
                {
                    MenuId = reader11.To<int>("MenuId"),
                    Count = reader11.To<ulong>("VisitedCount"),
                    Position = reader11.To<int>("Position")
                };
            };

            return Fetch<MostVisitedPage>(criteria, mapObjectFunc);
        }

        public void UpdateCount(ICriteria mostVisitedPage)
        {
            ExecuteCommand(mostVisitedPage);
        }

        public void UpdatePosition(ICriteria mostVisitedPage)
        {
            ExecuteCommand(mostVisitedPage);
        }

        public void Delete(ICriteria mostVisitedPage)
        {
            ExecuteCommand(mostVisitedPage);
        }
    }
}