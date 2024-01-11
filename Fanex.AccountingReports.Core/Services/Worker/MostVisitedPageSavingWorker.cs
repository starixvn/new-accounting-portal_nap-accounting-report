namespace Fanex.AccountingReports.Core.Services.Impl.Worker
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Core.Web;

    public class MostVisitedPageSavingWorker : BaseWorker
    {
        public MostVisitedPageSavingWorker(string threadName)
            : base(threadName)
        {
        }

        protected override void Run()
        {
            var intervalGetMostVisitedPage = 10;

            intervalGetMostVisitedPage = AppSettings.IntervalGetMostVisitedPage;

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

        private void SaveMostVisitedPageCount()
        {
            lock (CommonConstants.MostVisitedPageLockObject)
            {
                List<MostVisitedPage> mostVisitedPages = CacheExtension.Get<List<MostVisitedPage>>(CommonConstants.MistVisitedPageCacheKey);
                if (mostVisitedPages != null && mostVisitedPages.Count > 0)
                {
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

                    CacheExtension.Set(CommonConstants.MistVisitedPageCacheKey, new List<MostVisitedPage>());
                }
            }
        }
    }
}
