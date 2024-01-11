namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SetX_Info
    {
        public string Id { get; set; }

        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }
    }
}
