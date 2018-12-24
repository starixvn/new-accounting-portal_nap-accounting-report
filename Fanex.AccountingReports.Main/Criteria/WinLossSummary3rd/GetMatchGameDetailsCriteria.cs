namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using System.Linq;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Main.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchGameDetailsCriteria : CriteriaBase
    {
        // To determine type of this detail: SB, Racing, Virtual sports, Virtual racing
        private SportTypeWinLoss _type = SportTypeWinLoss.Unknown;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public int LiveIndicator { get; set; }

        public string ParentName { get; set; }

        [SpParam(SqlParamName = "drilldownbytype")]
        public int DrillDownByType { get; set; }

        public string SubProduct { get; set; }

        public SportTypeWinLoss Type
        {
            get
            {
                if (_type == SportTypeWinLoss.Unknown)
                {
                    if (SportType <= MainConstants.WinLossSummaryMaxSportbookId ||                        
                        SportType == SportContants.HorseRacingFixedOdds)
                    {
                        _type = SportTypeWinLoss.SportBook;
                    }
                    else if(SportType == 161 || SportType == 164)
                    {
                        _type = SportTypeWinLoss.NumberGame;
                    }
                    else if (SportType >= MainConstants.MinMetricGamingSuperLiveBetType && SportType <= MainConstants.MaxMetricGamingSuperLiveBetType)
                    {
                        _type = SportTypeWinLoss.SportBook;
                    }
                    else if (MainConstants.WinLossSummaryRacingSportIds.Contains(SportType))
                    {
                        _type = SportTypeWinLoss.Racing;
                    }
                    else if (MainConstants.WinLossSummaryVirtualRacingSportTypeIds.Contains(SportType))
                    {
                        _type = SportTypeWinLoss.VirtualRacing;
                    }
                    else if (MainConstants.WinLossSummaryVirtualSportIds.Contains(SportType))
                    {
                        _type = SportTypeWinLoss.VirtualSports;
                    }
                    else if (SportType == MainConstants.WinLossSummary3rdBullfightingSportId)
                    {
                        _type = SportTypeWinLoss.Bullfighting;
                    }
                }

                return _type;
            }
        }
    }
}
