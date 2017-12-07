namespace Fanex.AccountingReports.Misc.Services.Interface.DuplicateTickets
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Models.DuplicateTickets;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IDuplicateTicketsService
    {
        DuplicateTicketsInfo GetDuplicateTicketByUserName(GetDuplicateTicketsCriteria criteria);

        void ExportDuplicateTicketReport(GetDuplicateTicketsCriteria criteria);
    }
}
