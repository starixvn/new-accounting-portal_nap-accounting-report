// -----------------------------------------------------------------------
// <copyright file="GetDuplicateTicketsCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Criteria.DuplicateTickets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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