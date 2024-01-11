namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Sport
    {
        public Sport()
        {
            Sports = new List<Sport>();
        }

        public int SportType { get; set; }

        public string SportName { get; set; }

        public List<Sport> Sports { get; set; }
    }
}
