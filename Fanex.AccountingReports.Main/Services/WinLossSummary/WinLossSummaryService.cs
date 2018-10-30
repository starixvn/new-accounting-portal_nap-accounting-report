namespace Fanex.AccountingReports.Main.Services.Implement.WinLossSummary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.AccountingReports.Main.Models.WinLossSummary;
    using Fanex.AccountingReports.Main.Repositories.Implement;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossSummaryService : ServiceBase
    {
        private WinLossSummaryRepository _reportRepository;
        private Dictionary<object, string> _betTypeNameCache;
        private Dictionary<object, string> _sportTypeNameCache;

        public WinLossSummaryService()
            : this(null)
        {
        }

        public WinLossSummaryService(WinLossSummaryRepository reportRepository)
        {
            this._reportRepository = reportRepository ?? new WinLossSummaryRepository();
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossInfo GetReport(CriteriaBase criteria)
        {
            try
            {
                Hashtable outParam = new Hashtable();
                var rawData = this._reportRepository.GetReport(criteria, ref outParam);
                var report = new WinLossInfo
                {
                    GrandTotal = new WinLossInfo(),
                    ChildsInfo = new List<WinLossInfo>()
                };

                var detail = Converter.ToInt(outParam["_details"]);
                var sportTypeList = rawData.Select(data => Converter.ToInt(data.SportType))
                                             .ToList();
                CacheRef.SaveCacheRefResponse(sportTypeList, CacheRef.GetSportName, ref this._sportTypeNameCache);

                var betTypeList = rawData.Select(data => Converter.ToInt(data.bType))
                                         .ToList();
                CacheRef.SaveCacheRefResponse(betTypeList, CacheRef.GetBettype, ref this._betTypeNameCache);

                var sportTypeGroups = rawData.GroupBy(data => data.SportType);
                foreach (var sportTypeGroup in sportTypeGroups)
                {
                    var sportInfo = new WinLossInfo
                    {
                        Id = Converter.ToInt(sportTypeGroup.Key),
                        GrandTotal = new WinLossInfo(),
                        ChildsInfo = new List<WinLossInfo>()
                    };

                    if (sportInfo.Id == MainConstants.WinLossSummaryTotalSportId)
                    {
                        report.GrandTotal.Name = Labels.Sum.ToUpper();
                        continue;
                    }

                    sportInfo.Name = this.GetSportName(sportInfo.Id);
                    foreach (var data in sportTypeGroup)
                    {
                        var info = new WinLossInfo
                        {
                            Id = Converter.ToInt(data.bType),
                            WinLoss = Converter.ToDecimal(data.WinLost),
                            Position = Converter.ToDecimal(data.Position),
                            Commission = Converter.ToDecimal(data.Commission),
                            LiveType = Converter.ToInt(data.liveindicator),
                            Status = Converter.ToString2(data.Status),
                            IBC = Converter.ToInt(data.ibc)
                        };

                        if (info.Id == MainConstants.WinLossSummaryTotalBetTypeId)
                        {
                            continue;
                        }

                        info.Name = this._betTypeNameCache[info.Id];
                        info.LineTotal = info.WinLoss + info.Position + info.Commission;
                        sportInfo.GrandTotal.Add(info);
                        info = this.FormatWinLossInfo(info);
                        sportInfo.ChildsInfo.Add(info);
                    }

                    report.GrandTotal.Add(sportInfo.GrandTotal);
                    sportInfo.GrandTotal = this.FormatWinLossInfo(sportInfo.GrandTotal);
                    report.ChildsInfo.Add(sportInfo);
                }

                SessionManager.Set<string>(MainConstants.TraderPerformanceDetailReportSessionKey, null);
                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            SessionManager.Set<string>(MainConstants.TraderPerformanceDetailReportSessionKey, null);
            return null;
        }

        /// <summary>
        /// Gets the name of the sport.
        /// </summary>
        /// <param name="sportId">The sport identifier.</param>
        /// <returns>The name of the sport.</returns>
        public string GetSportName(int sportId)
        {
            var sportName = string.Empty;
            switch (sportId)
            {
                case MainConstants.WinLossSummaryMixParlaySportId:
                    sportName = Labels.Mix_Parlay;
                    break;

                case MainConstants.WinLossSummarySystemParlaySportId:
                    sportName = Labels.System_Parlay;
                    break;

                default:
                    sportName = this._sportTypeNameCache[sportId];
                    break;
            }

            return sportName;
        }

        /// <summary>
        /// Formats the win loss information.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The formatted win loss.</returns>
        private WinLossInfo FormatWinLossInfo(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.WinLoss_Str = winLoss.WinLoss.Format(2);
            winLoss.Position_Str = winLoss.Position.Format(2);
            winLoss.Commission_Str = winLoss.Commission.Format(2);
            winLoss.LineTotal_Str = winLoss.LineTotal.Format(2);
            return winLoss;
        }
    }
}