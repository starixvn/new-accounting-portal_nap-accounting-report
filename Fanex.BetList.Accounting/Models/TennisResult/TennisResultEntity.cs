namespace Fanex.BetList.Accounting.Models
{
    public class TennisResultEntity
    {
        public string MatchId { get; set; }
 
        public string ResourceId { get; set; }

        public string HomeValue { get; set; }

        public string AwayValue { get; set; }

        public int ResultStatusId { get; set; }

        public int SettlementStatusId { get; set; }
    }
}
