namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>    
    public class Match
    {
        public Match()
        {
            MatchCode = string.Empty;
            Home = new BaseModel();
            Away = new BaseModel();
            Sport = new BaseModel();
        }

        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public string ShowTime { get; set; }

        public BaseModel Home { get; set; }

        public BaseModel Away { get; set; }

        public BaseModel Sport { get; set; }
    }
}
