namespace Fanex.AccountingReports.Core.Models
{
    using Fanex.AccountingReports.Core.Services.Impl;

    public class League
    {
        public int LeagueId { get; set; }

        public string LeagueName 
        {
            get
            {
                return CacheRef.GetLeagueName(LeagueId);
            }
        }
    }
}