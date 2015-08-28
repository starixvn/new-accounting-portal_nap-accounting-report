// -----------------------------------------------------------------------
// <copyright file="ResultPopupCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ResultPopupCriteria : CriteriaBase 
    {
        [SpParam(SqlParamName = "MatchID")]
        public int MatchId { get; set; }

        public int? Bettype { get; set; }

        public double? BetId { get; set; }

        public int? RaceNo { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_BetList_SelMatchResult";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}