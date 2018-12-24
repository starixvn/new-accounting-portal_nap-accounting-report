using System.Collections.Generic;

namespace Fanex.AccountingReports.Gaming.Models
{
    public class NumberGameSport
    {
        public string SportName { get; set; }

        public List<NumberGameBettype> Bettypes { get; set; }
    }
}