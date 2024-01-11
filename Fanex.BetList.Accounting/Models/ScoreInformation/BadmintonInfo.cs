namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    [Serializable]
    public class BadmintonInfo : ScoreInfo
    {
        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }

        [ScriptIgnore]
        public int HomeTotalGameScore { get; set; }

        [ScriptIgnore]
        public int AwayTotalGameScore { get; set; }

        public string HomeTotalGameScore_Str { get; set; }

        public string AwayTotalGameScore_Str { get; set; }
    }
}
