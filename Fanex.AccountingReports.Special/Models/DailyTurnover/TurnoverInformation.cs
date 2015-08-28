// -----------------------------------------------------------------------
// <copyright file="TurnoverInformation.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Models.DailyTurnover
{
    using System;

    /// <summary>
    /// The information of turnover.
    /// </summary>
    [Serializable]
    public class TurnoverInformation
    {
        public decimal TotalTurnover { get; set; }

        public decimal SuperTurnover { get; set; }

        public decimal MasterTurnover { get; set; }

        public decimal AgentTurnover { get; set; }

        public decimal IBCTurnover { get; set; }

        public bool IsActual { get; set; }
    }
}
