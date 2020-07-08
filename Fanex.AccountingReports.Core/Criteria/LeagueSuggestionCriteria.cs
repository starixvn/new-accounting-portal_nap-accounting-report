using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.Core.Criteria
{
    public class LeagueSuggestionCriteria : CriteriaBase
    {
        public string SportBettype { get; set; }

        public string SearchText { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetLeagueSuggestion";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}