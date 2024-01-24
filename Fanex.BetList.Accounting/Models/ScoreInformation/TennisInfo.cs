namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    [Serializable]
    public class TennisInfo : ScoreInfo
    {
        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }

        public SetX_Info SetXInformation { get; set; }

        [ScriptIgnore]
        public int HomeTotalGamesScore { get; set; }

        [ScriptIgnore]
        public int AwayTotalGamesScore { get; set; }

        #region DTOField

        public string HomeTotalGamesScore_Str { get; set; }

        public string AwayTotalGamesScore_Str { get; set; }

        #endregion DTOField
    }
}
