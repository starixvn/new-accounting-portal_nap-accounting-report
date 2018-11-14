namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MostVisitedPagesService : ServiceBase
    {
        private readonly MostVisitedPagesRepository _mostVisitedPagesRepository;
        private readonly MenuService _menuService;

        public MostVisitedPagesService()
        {
            this._mostVisitedPagesRepository =
                new MostVisitedPagesRepository();
            this._menuService = new MenuService();
        }

        /// <summary>
        /// Gets the most visited pages.
        /// </summary>
        /// <returns>The most visited pages.</returns>
        public IEnumerable<MostVisitedPage> GetMostVisitedPages()
        {
            try
            {
                if (AppSettings.IsDisableMostvisitedPage)
                {
                    return null;
                }

                var rawData = this._mostVisitedPagesRepository.FetchMostVisitedPages(new SelectMostVisitedPageCriteria());
                var mostvisitedPages = new List<MostVisitedPage>();
                foreach (var row in rawData)
                {
                    var menu = this._menuService.GetMenuById(Converter.ToInt(row.MenuId));
                    if (menu != null)
                    {
                        mostvisitedPages.Add(new MostVisitedPage
                        {
                            MenuName = menu.Name,
                            MenuURL = menu.Url.Trim(),
                            MenuId = menu.Id,
                            Count = row.Count,
                            Position = Converter.ToInt(row.Position),
                            Title = (menu.Parent != null
                                        ? menu.Parent.Name + MenuService.UrlSeparator
                                        : string.Empty) + menu.Name
                        });
                    }
                }

                this.SortMostVisitedPages(ref mostvisitedPages);
                return mostvisitedPages;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Updates the count.
        /// </summary>
        /// <param name="mostVisitedPage">The most visited page.</param>
        public void UpdateCount(MostVisitedPage mostVisitedPage)
        {
            try
            {
                if (AppSettings.IsDisableMostvisitedPage || mostVisitedPage == null)
                {
                    return;
                }

                lock (CommonConstants.MostVisitedPageLockObject)
                {
                    ////Update to Memory
                    List<MostVisitedPage> mostVisitedPages = CacheExtension.Get<List<MostVisitedPage>>(CommonConstants.MistVisitedPageCacheKey);
                    if (mostVisitedPages == null)
                    {
                        mostVisitedPages = new List<MostVisitedPage>();
                    }

                    var mostVisitedPagesCount = mostVisitedPages.Count;
                    var isExist = false;
                    for (int i = 0; i < mostVisitedPagesCount; i++)
                    {
                        if (mostVisitedPages[i] != null &&
                            mostVisitedPages[i].MenuId == mostVisitedPage.MenuId)
                        {
                            mostVisitedPages[i].Count++;
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist && AppSession.CurrentUser != null)
                    {
                        mostVisitedPages.Add(new MostVisitedPage
                        {
                            UserId = AppSession.CurrentUser.Id,
                            MenuId = mostVisitedPage.MenuId
                        });
                    }

                    // Save to cache.
                    CacheExtension.Set(CommonConstants.MistVisitedPageCacheKey, mostVisitedPages);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        /// <param name="mostVisitedPage">The most visited page.</param>
        public void UpdatePosition(ICriteria mostVisitedPage)
        {
            try
            {
                if (AppSettings.IsDisableMostvisitedPage)
                {
                    return;
                }

                this._mostVisitedPagesRepository.UpdatePosition(mostVisitedPage);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Deletes the specified most visited page.
        /// </summary>
        /// <param name="mostVisitedPage">The most visited page.</param>
        public void Delete(ICriteria mostVisitedPage)
        {
            try
            {
                if (AppSettings.IsDisableMostvisitedPage)
                {
                    return;
                }

                this._mostVisitedPagesRepository.Delete(mostVisitedPage);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private bool IsInRange(int position, int maxPages)
        {
            return position >= 1 && position <= maxPages;
        }

        /// <summary>
        /// Sorts the most visited pages.
        /// </summary>
        /// <param name="mostVisitedPages">The most visited pages.</param>
        private void SortMostVisitedPages(ref List<MostVisitedPage> mostVisitedPages)
        {
            try
            {
                if (mostVisitedPages == null)
                {
                    return;
                }

                mostVisitedPages = mostVisitedPages.Take(SelectMostVisitedPageCriteria.MaxPages)
                                                   .ToList();

                var pinSite = mostVisitedPages.Where(mvp =>
                    this.IsInRange(mvp.Position, SelectMostVisitedPageCriteria.MaxPages)).ToList();
                var unpinSite = mostVisitedPages.Where(mvp =>
                    !this.IsInRange(mvp.Position, SelectMostVisitedPageCriteria.MaxPages)).ToList();
                mostVisitedPages = unpinSite;

                for (int i = 0; i < pinSite.Count; i++)
                {
                    var mostVisitedPage = pinSite[i];
                    if (mostVisitedPage.Position <= mostVisitedPages.Count)
                    {
                        mostVisitedPages.Insert(mostVisitedPage.Position - 1, mostVisitedPage);
                    }
                    else
                    {
                        mostVisitedPages.Add(mostVisitedPage);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}