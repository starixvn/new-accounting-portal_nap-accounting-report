// -----------------------------------------------------------------------
// <copyright file="LeagueMatch.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueMatch : BaseModel
    {
        public LeagueMatch()
        {
            Matches = new List<Match>();
        }

        public List<Match> Matches { get; set; }
    }
}
