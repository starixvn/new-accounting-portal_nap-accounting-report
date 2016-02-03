// --------------------------------------------------------------------------
// <copyright file="MostVisitedPageSavingWorker.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.17">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Services.Impl.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Core.Web;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MostVisitedPageSavingWorker : BaseWorker
    {
        public MostVisitedPageSavingWorker(string threadName)
            : base(threadName)
        {
        }

        /// <summary>
        /// Run Main Function.
        /// </summary>
        protected override void Run()
        {
            var intervalGetMostVisitedPage = 10;
            try
            {
                intervalGetMostVisitedPage = AppSettings.IntervalGetMostVisitedPage;
            }
            catch (Exception)
            {
                intervalGetMostVisitedPage = 10;
            }

            while (true)
            {
                Thread.Sleep(intervalGetMostVisitedPage * 1000);
                if (this.IsStop)
                {
                    break;
                }

                this.SaveMostVisitedPageCount();
            }
        }

        /// <summary>
        /// Saves the most visited page count.
        /// </summary>
        private void SaveMostVisitedPageCount()
        {
            try
            {
                lock (CommonConstants.MostVisitedPageLockObject)
                {
                    List<MostVisitedPage> mostVisitedPages = CacheExtension.Get<List<MostVisitedPage>>(CommonConstants.MistVisitedPageCacheKey);
                    if (mostVisitedPages != null && mostVisitedPages.Count > 0)
                    {
                        //// Save to Database
                        var criteria = new UpdateMostVisitedPageCountCriteria
                        {
                            CustId = mostVisitedPages.First().UserId,
                            MenuId = string.Join<string>(
                                CommonConstants.SPParamSeparator,
                                mostVisitedPages.Select(mvp => mvp.MenuId.ToString())),
                            Count = string.Join<string>(
                                CommonConstants.SPParamSeparator,
                                mostVisitedPages.Select(mvp => mvp.Count.ToString())),
                        };

                        var repository = new MostVisitedPagesRepository();
                        repository.UpdateCount(criteria);

                        //// Update Cache
                        CacheExtension.Set(CommonConstants.MistVisitedPageCacheKey, new List<MostVisitedPage>());
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
