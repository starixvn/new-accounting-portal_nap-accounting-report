namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class BasketballInfo : TwoTimesMatchInfo
    {
        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }
    }
}
