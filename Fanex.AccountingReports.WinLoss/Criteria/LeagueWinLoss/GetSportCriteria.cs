using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    public class GetSportCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "WinLoss_LeagueWinLoss_GetSports";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
