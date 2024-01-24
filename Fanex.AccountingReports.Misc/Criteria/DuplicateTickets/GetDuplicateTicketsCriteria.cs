namespace Fanex.AccountingReports.Misc.Criteria.DuplicateTickets
{
    using System;
    using Fanex.Data.Repository;

    public class GetDuplicateTicketsCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public short CheckTime { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_DuplicateTickets_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
