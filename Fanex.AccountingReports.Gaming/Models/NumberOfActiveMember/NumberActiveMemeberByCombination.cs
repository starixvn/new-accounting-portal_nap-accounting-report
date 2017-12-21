namespace Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class NumberActiveMemeberByCombination
    {
        public NumberActiveMemeberByCombination()
        {
            NumberActiveMemberByProduct = new List<NumberOfActiveMember>();
        }

        public string CombinationName { get; set; }

        public List<NumberOfActiveMember> NumberActiveMemberByProduct { get; set; }
    }
}