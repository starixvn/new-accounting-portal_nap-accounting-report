namespace Fanex.BetList.Accounting.Models
{
    using System.Collections.Generic;

    public class TennisSetResult
    {
        public TennisSetResult()
        {
            this.GameResults = new List<TennisGameResult>();
        }

        public List<TennisGameResult> GameResults { get; set; }

        public TennisResultEntity SetDetail { get; set; }

        public int MaxGame { get; set; }
    }
}
