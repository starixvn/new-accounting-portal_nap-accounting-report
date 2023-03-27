using System.Collections.Generic;

namespace Fanex.AccountingReports.Misc.Models.CreditMovement
{
    public class SiteGroup
    {
        public string GroupName { get; set; }

        public List<Site> Sites { get; set; }
    }
}
