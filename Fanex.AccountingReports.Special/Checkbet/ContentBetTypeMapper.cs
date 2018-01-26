using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.AccountingReports.Special.Checkbet
{
    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
    public class ContentBetTypeMapper
    {
        private const string ContenBetTeamBetType401To404 = "ContenBetTeamBetType401To404";
        private const string ContenBetTeamBetType406 = "ContenBetTeamBetType406";
        private const string ContenBetTeamBetType407 = "ContenBetTeamBetType407";
        private const string ContenBetTeamBetType411 = "ContenBetTeamBetType411";
        private const string ContenBetTeamBetType412And429 = "ContenBetTeamBetType412And429";
        private const string ContenBetTeamBetType417 = "ContenBetTeamBetType417";
        private const string ContenBetTeamBetType418 = "ContenBetTeamBetType418";
        private const string ContenBetTeamBetType419To421 = "ContenBetTeamBetType419To421";
        private const string ContenBetTeamBetType422To423 = "ContenBetTeamBetType422To423";
        private const string ContenBetTeamBetType424 = "ContenBetTeamBetType424";
        private const string ContenBetTeamBetType442To444 = "ContenBetTeamBetType442To444";
        private const string ContenBetTeamBetType445To447 = "ContenBetTeamBetType445To447";
        private const string ContenBetTeamBetTypeOddEven = "ContenBetTeamBetTypeOddEven";
        private const string ContenBetTeamBetTypeYesNo = "ContenBetTeamBetTypeYesNo";
        private const string ContenBetTeamBetTypeAsBetTeam = "ContenBetTeamBetTypeAsBetTeam";

        private const string ContenBetTeamBetType221To225 = "ContenBetTeamBetType221To225";

        private const string ContenBetTeamBetType603To607And609And612And613And617 = "ContenBetTeamBetType603To607And609And612And613And617";
        private const string ContenBetTeamBetType610And615And616 = "ContenBetTeamBetType610And615And616";
        private const string ContenBetTeamBetType611 = "ContenBetTeamBetType611";


        private readonly Dictionary<int, string> groupBettypes;

        public ContentBetTypeMapper()
        {
            groupBettypes = new Dictionary<int, string>()
            {
                { 401, ContenBetTeamBetType401To404 },
                { 402, ContenBetTeamBetType401To404 },
                { 403, ContenBetTeamBetType401To404 },
                { 404, ContenBetTeamBetType401To404 },
                { 461, ContenBetTeamBetType401To404 },
                { 462, ContenBetTeamBetType401To404 },
                { 463, ContenBetTeamBetType401To404 },
                { 464, ContenBetTeamBetType401To404 },
                { 405, ContenBetTeamBetTypeAsBetTeam },
                { 406, ContenBetTeamBetType406 },
                { 407, ContenBetTeamBetType407 },
                { 409, ContenBetTeamBetType407 },
                { 411, ContenBetTeamBetType411 },
                { 412, ContenBetTeamBetType412And429 },
                { 413, ContenBetTeamBetTypeAsBetTeam },
                { 414, ContenBetTeamBetTypeAsBetTeam },
                { 416, ContenBetTeamBetTypeAsBetTeam },
                { 417, ContenBetTeamBetType417 },
                { 418, ContenBetTeamBetType418 },
                { 419, ContenBetTeamBetType419To421 },
                { 420, ContenBetTeamBetType419To421 },
                { 421, ContenBetTeamBetType419To421 },
                { 422, ContenBetTeamBetType422To423 },
                { 423, ContenBetTeamBetType422To423 },
                { 424, ContenBetTeamBetType424 },
                { 425, ContenBetTeamBetType422To423 },
                { 427, ContenBetTeamBetTypeYesNo },
                { 428, ContenBetTeamBetTypeOddEven },
                { 429, ContenBetTeamBetType412And429 },
                { 432, ContenBetTeamBetType411 },
                { 433, ContenBetTeamBetTypeYesNo },
                { 434, ContenBetTeamBetTypeYesNo },
                { 435, ContenBetTeamBetTypeYesNo },
                { 436, ContenBetTeamBetTypeYesNo },
                { 437, ContenBetTeamBetTypeYesNo },
                { 438, ContenBetTeamBetTypeYesNo },
                { 439, ContenBetTeamBetTypeYesNo },
                { 440, ContenBetTeamBetTypeYesNo },
                { 441, ContenBetTeamBetTypeYesNo },
                { 442, ContenBetTeamBetType442To444 },
                { 443, ContenBetTeamBetType442To444 },
                { 444, ContenBetTeamBetType442To444 },
                { 445, ContenBetTeamBetType445To447 },
                { 446, ContenBetTeamBetType445To447 },
                { 447, ContenBetTeamBetType445To447 },

                { 221, ContenBetTeamBetType221To225 },
                { 222, ContenBetTeamBetType221To225 },
                { 223, ContenBetTeamBetType221To225 },
                { 224, ContenBetTeamBetType221To225 },
                { 225, ContenBetTeamBetType221To225 },

                { 603, ContenBetTeamBetType603To607And609And612And613And617 },
                { 604, ContenBetTeamBetType603To607And609And612And613And617 },
                { 605, ContenBetTeamBetType603To607And609And612And613And617 },
                { 606, ContenBetTeamBetType603To607And609And612And613And617 },
                { 607, ContenBetTeamBetType603To607And609And612And613And617 },
                { 609, ContenBetTeamBetType603To607And609And612And613And617 },
                { 610, ContenBetTeamBetType610And615And616 },
                { 611, ContenBetTeamBetType611 },
                { 612, ContenBetTeamBetType603To607And609And612And613And617 },
                { 613, ContenBetTeamBetType603To607And609And612And613And617 },
                { 615, ContenBetTeamBetType610And615And616 },
                { 616, ContenBetTeamBetType610And615And616 },
                { 617, ContenBetTeamBetType603To607And609And612And613And617 },

            };
        }

        public string GetContentBetchoiceToShowOf(int bettype)
        {
            return groupBettypes.ContainsKey(bettype) ? groupBettypes[bettype] : string.Empty;
        }
    }
}
