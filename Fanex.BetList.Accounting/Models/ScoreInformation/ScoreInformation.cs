namespace Fanex.BetList.Accounting.Models.ScoreInformation
{
    using System;

    [Serializable]
    public class ScoreInformation
    {
        public int MatchId { get; set; }

        public string TraderName { get; set; }

        public string MatchInformation { get; set; }
    }
}
