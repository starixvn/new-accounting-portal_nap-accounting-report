using Fanex.AccountingReports.Misc.Criteria.DuplicateTickets;
using Fanex.Common;
using Fanex.Data;

namespace Fanex.AccountingReports.Misc.Services.Implement.DuplicateTickets
{
    public class DuplicateTicketsCashOutServiceImp
    {
        public bool IsDuplicateTicketsCashOut(int betTypeId, string transIds)
        {
            if (betTypeId != 911)
            {
                return false;
            }

            var criteria = new DuplicateTicketsCheckCashOutCriteria { TransIds = transIds };
            var isDuplicate = false;
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
                var outoutResult = db.GetOutputParams();
                isDuplicate = Converter.To<bool>(outoutResult["isduplicated"]);
            }

            return isDuplicate;
        }
    }
}