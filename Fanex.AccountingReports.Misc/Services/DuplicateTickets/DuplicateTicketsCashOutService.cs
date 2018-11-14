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
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                db.ExecuteNonQuery();
                var outoutResult = db.GetOuputParams();
                isDuplicate = Converter.To<bool>(outoutResult["isduplicated"]);
            }

            return isDuplicate;
        }
    }
}