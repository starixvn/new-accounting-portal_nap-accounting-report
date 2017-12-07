namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportLeague : BaseModel
    {
        public SportLeague()
        {
            Leagues = new List<League>();
        }

        public List<League> Leagues { get; set; }
    }
}
