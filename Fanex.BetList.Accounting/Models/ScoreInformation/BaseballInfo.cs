namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BaseballInfo : TwoTimesMatchInfo
    {
        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }

        public string HomeEIScore { get; set; }

        public string AwayEIScore { get; set; }
    }
}
