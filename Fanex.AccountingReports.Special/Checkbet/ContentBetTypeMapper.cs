using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.AccountingReports.Special.Checkbet
{
    [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
    public class ContentBetTypeMapper
    {
        private const string ContentBetTeamBetType401To404 = "ContentBetTeamBetType401To404";
        private const string ContentBetTeamBetType406 = "ContentBetTeamBetType406";
        private const string ContentBetTeamBetType407 = "ContentBetTeamBetType407";
        private const string ContentBetTeamBetType411 = "ContentBetTeamBetType411";
        private const string ContentBetTeamBetType412And429 = "ContentBetTeamBetType412And429";
        private const string ContentBetTeamBetType417 = "ContentBetTeamBetType417";
        private const string ContentBetTeamBetType418 = "ContentBetTeamBetType418";
        private const string ContentBetTeamBetType419To421 = "ContentBetTeamBetType419To421";
        private const string ContentBetTeamBetType422To423 = "ContentBetTeamBetType422To423";
        private const string ContentBetTeamBetType424 = "ContentBetTeamBetType424";
        private const string ContentBetTeamBetType442To444 = "ContentBetTeamBetType442To444";
        private const string ContentBetTeamBetType445To447 = "ContentBetTeamBetType445To447";
        private const string ContentBetTeamBetTypeOddEven = "ContentBetTeamBetTypeOddEven";
        private const string ContentBetTeamBetTypeYesNo = "ContentBetTeamBetTypeYesNo";
        private const string ContentBetTeamBetTypeAsBetTeam = "ContentBetTeamBetTypeAsBetTeam";

        private const string ContentBetTeamBetType223To227 = "ContentBetTeamBetType223To227";

        private const string ContentBetTeamBetType603To607And609And612And613And617 = "ContentBetTeamBetType603To607And609And612And613And617";
        private const string ContentBetTeamBetType610And615And616 = "ContentBetTeamBetType610And615And616";
        private const string ContentBetTeamBetType611 = "ContentBetTeamBetType611";
        private const string ContentBetTeamBetType221To222 = "ContentBetTeamBetType221To222";
        private const string ContentBetTeamBetType38 = "ContentBetTeamBetType38";

        private readonly Dictionary<int, string> groupBettypes;

        public ContentBetTypeMapper()
        {
            groupBettypes = new Dictionary<int, string>()
            {
                { 401, ContentBetTeamBetType401To404 },
                { 402, ContentBetTeamBetType401To404 },
                { 403, ContentBetTeamBetType401To404 },
                { 404, ContentBetTeamBetType401To404 },
                { 461, ContentBetTeamBetType401To404 },
                { 462, ContentBetTeamBetType401To404 },
                { 463, ContentBetTeamBetType401To404 },
                { 464, ContentBetTeamBetType401To404 },
                { 405, ContentBetTeamBetTypeAsBetTeam },
                { 406, ContentBetTeamBetType406 },
                { 407, ContentBetTeamBetType407 },
                { 409, ContentBetTeamBetType407 },
                { 411, ContentBetTeamBetType411 },
                { 412, ContentBetTeamBetType412And429 },
                { 413, ContentBetTeamBetTypeAsBetTeam },
                { 414, ContentBetTeamBetTypeAsBetTeam },
                { 416, ContentBetTeamBetTypeAsBetTeam },
                { 417, ContentBetTeamBetType417 },
                { 418, ContentBetTeamBetType418 },
                { 419, ContentBetTeamBetType419To421 },
                { 420, ContentBetTeamBetType419To421 },
                { 421, ContentBetTeamBetType419To421 },
                { 422, ContentBetTeamBetType422To423 },
                { 423, ContentBetTeamBetType422To423 },
                { 424, ContentBetTeamBetType424 },
                { 425, ContentBetTeamBetType422To423 },
                { 427, ContentBetTeamBetTypeYesNo },
                { 428, ContentBetTeamBetTypeOddEven },
                { 429, ContentBetTeamBetType412And429 },
                { 432, ContentBetTeamBetType411 },
                { 433, ContentBetTeamBetTypeYesNo },
                { 434, ContentBetTeamBetTypeYesNo },
                { 435, ContentBetTeamBetTypeYesNo },
                { 436, ContentBetTeamBetTypeYesNo },
                { 437, ContentBetTeamBetTypeYesNo },
                { 438, ContentBetTeamBetTypeYesNo },
                { 439, ContentBetTeamBetTypeYesNo },
                { 440, ContentBetTeamBetTypeYesNo },
                { 441, ContentBetTeamBetTypeYesNo },
                { 442, ContentBetTeamBetType442To444 },
                { 443, ContentBetTeamBetType442To444 },
                { 444, ContentBetTeamBetType442To444 },
                { 445, ContentBetTeamBetType445To447 },
                { 446, ContentBetTeamBetType445To447 },
                { 447, ContentBetTeamBetType445To447 },

                { 221, ContentBetTeamBetType221To222 },
                { 222, ContentBetTeamBetType221To222 },
                { 223, ContentBetTeamBetType223To227 },
                { 224, ContentBetTeamBetType223To227 },
                { 225, ContentBetTeamBetType223To227 },
                { 226, ContentBetTeamBetType223To227 },
                { 227, ContentBetTeamBetType223To227 },

                { 603, ContentBetTeamBetType603To607And609And612And613And617 },
                { 604, ContentBetTeamBetType603To607And609And612And613And617 },
                { 605, ContentBetTeamBetType603To607And609And612And613And617 },
                { 606, ContentBetTeamBetType603To607And609And612And613And617 },
                { 607, ContentBetTeamBetType603To607And609And612And613And617 },
                { 609, ContentBetTeamBetType603To607And609And612And613And617 },
                { 610, ContentBetTeamBetType610And615And616 },
                { 611, ContentBetTeamBetType611 },
                { 612, ContentBetTeamBetType603To607And609And612And613And617 },
                { 613, ContentBetTeamBetType603To607And609And612And613And617 },
                { 615, ContentBetTeamBetType610And615And616 },
                { 616, ContentBetTeamBetType610And615And616 },
                { 617, ContentBetTeamBetType603To607And609And612And613And617 },
                { 38, ContentBetTeamBetType38 }
            };
        }

        public string GetContentBetchoiceToShowOf(int bettype)
        {
            return groupBettypes.ContainsKey(bettype) ? groupBettypes[bettype] : string.Empty;
        }
    }
}
