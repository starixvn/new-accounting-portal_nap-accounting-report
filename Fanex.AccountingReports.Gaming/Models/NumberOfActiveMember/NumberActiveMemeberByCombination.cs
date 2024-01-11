namespace Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember
{
    using System;
    using System.Collections.Generic;

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
