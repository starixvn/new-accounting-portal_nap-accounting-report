using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fanex.AccountingReports.Special.Services.Implement
{
    public class GetBetTeamEnhanced
    {
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public string GetStringFollowBetType(string homeTeam, string awayTeam, int betType, string betTeam, decimal hdp1)
        {
            var betTypeName = new Dictionary<int, string>();
            betTeam = betTeam.ToUpper();
            hdp1 = decimal.Round(hdp1, 2, System.MidpointRounding.AwayFromZero);

            betTypeName.Add(401, GetStringBetTypeFrom401to404(betTeam, hdp1));
            betTypeName.Add(402, GetStringBetTypeFrom401to404(betTeam, hdp1));
            betTypeName.Add(403, GetStringBetTypeFrom401to404(betTeam, hdp1));
            betTypeName.Add(404, GetStringBetTypeFrom401to404(betTeam, hdp1));
            betTypeName.Add(405, betTeam);
            betTypeName.Add(406, GetStringBetType406(betTeam));
            betTypeName.Add(407, GetStringBetType407(betTeam));
            betTypeName.Add(408, GetStringBetType408(betTeam, homeTeam, awayTeam));
            betTypeName.Add(409, GetStringBetType407(betTeam));
            betTypeName.Add(410, GetStringBetType410(betTeam, homeTeam, awayTeam));
            betTypeName.Add(411, GetStringBetType411(betTeam, homeTeam, awayTeam));
            betTypeName.Add(412, GetStringBetType412And429(betTeam));
            betTypeName.Add(413, betTeam);
            betTypeName.Add(414, betTeam);
            betTypeName.Add(415, GetStringBetType415(betTeam, homeTeam, awayTeam));
            betTypeName.Add(416, betTeam);
            betTypeName.Add(417, GetStringBetType417(betTeam, homeTeam, awayTeam));
            betTypeName.Add(418, GetStringBetType418(betTeam, hdp1));
            betTypeName.Add(419, GetStringBetType419To421(betTeam));
            betTypeName.Add(420, GetStringBetType419To421(betTeam));
            betTypeName.Add(421, GetStringBetType419To421(betTeam));
            betTypeName.Add(422, GetStringBetType422To423(betTeam));
            betTypeName.Add(423, GetStringBetType422To423(betTeam));
            betTypeName.Add(424, GetStringBetType424(betTeam));
            betTypeName.Add(425, GetStringBetType422To423(betTeam));
            betTypeName.Add(426, GetStringBetType426(betTeam, homeTeam, awayTeam));
            betTypeName.Add(427, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(428, GetStringBetTypeOddEven(betTeam));
            betTypeName.Add(429, GetStringBetType412And429(betTeam));
            betTypeName.Add(431, GetStringBetTypeDoubleChance(betTeam, homeTeam, awayTeam));
            betTypeName.Add(432, GetStringBetType411(betTeam, homeTeam, awayTeam));
            betTypeName.Add(433, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(434, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(435, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(436, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(437, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(438, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(439, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(440, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(441, GetStringBetTypeYesNo(betTeam));
            betTypeName.Add(442, GetStringBetType442To444(betTeam));
            betTypeName.Add(443, GetStringBetType442To444(betTeam));
            betTypeName.Add(444, GetStringBetType442To444(betTeam));
            betTypeName.Add(445, GetStringBetType445To447(betTeam));
            betTypeName.Add(446, GetStringBetType445To447(betTeam));
            betTypeName.Add(447, GetStringBetType445To447(betTeam));

            return betTypeName.ContainsKey(betType) == true ? betTypeName[betType] : string.Empty;
        }

        private string GetStringBetTypeFrom401to404(string betTeam, decimal hdp1)
        {
            return betTeam == "O" ? "Over " + hdp1.ToString() : "Under " + hdp1.ToString();
        }

        private string GetStringBetType406(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("G0", "0 Goals");
            team.Add("G1", "1 Goal");
            team.Add("G2", "2 Goals");
            team.Add("G3", "3 Goals");
            team.Add("G4", "4 Goals");
            team.Add("G5", "5 Goals");
            team.Add("G6", "6 Goals &Over");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType407(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("G0", "0 Goals");
            team.Add("G1", "1 Goal");
            team.Add("G2", "2 Goals");
            team.Add("G3", "3 Goals &Over");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType408(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("H1", homeTeam + " to Win by 1 Goal");
            team.Add("H2", homeTeam + " to Win by 2 Goals");
            team.Add("H3", homeTeam + " to Win by 3up Goals");
            team.Add("D", "S.Draw");
            team.Add("A1", awayTeam + " to Win by 1 Goal");
            team.Add("A2", awayTeam + " to Win by 2 Goals");
            team.Add("A3", awayTeam + " to Win by 3up Goals");
            team.Add("NG", "No Goal");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType410(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("12", homeTeam + " or " + awayTeam);
            team.Add("1X", homeTeam + " or Draw");
            team.Add("2X", homeTeam + " or Draw");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType411(string betTeam, string homeTeam, string awayTeam)
        {
            return betTeam == "H" ? homeTeam : awayTeam;
        }

        private string GetStringBetType415(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("HH", homeTeam + "/" + homeTeam);
            team.Add("DH", "Draw" + "/" + homeTeam);
            team.Add("AH", awayTeam + "/" + homeTeam);
            team.Add("HA", homeTeam + "/" + awayTeam);
            team.Add("DA", "Draw" + "/" + awayTeam);
            team.Add("AA", awayTeam + "/" + awayTeam);
            team.Add("NO", "None");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType417(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("YH", "Yes" + "/" + homeTeam);
            team.Add("YD", "Yes/Draw");
            team.Add("YA", "Yes" + "/" + awayTeam);
            team.Add("NH", "No" + "/" + homeTeam);
            team.Add("ND", "No/Draw");
            team.Add("NA", "No" + "/" + awayTeam);

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType418(string betTeam, decimal hdp1)
        {
            var team = new Dictionary<string, string>();
            team.Add("YO", "Yes & Over" + " " + hdp1.ToString());
            team.Add("YU", "Yes & Under" + " " + hdp1.ToString());
            team.Add("NO", "No & Over" + " " + hdp1.ToString());
            team.Add("NU", "No & Under" + " " + hdp1.ToString());

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType419To421(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("1H", "1st Half");
            team.Add("2H", "2nd Half");
            team.Add("N", "Neither");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType422To423(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("H", "Home");
            team.Add("A", "Away");
            team.Add("N", "Neither");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType424(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("S", "Shot");
            team.Add("H", "Header");
            team.Add("P", "Penalty");
            team.Add("FK", "Free Kick");
            team.Add("OG", "Own Goal");
            team.Add("NG", "No Goal");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType426(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("H1", homeTeam + " to Win by 1 Goal");
            team.Add("H2+", homeTeam + " to Win by 2up Goals");
            team.Add("D", "S.Draw");
            team.Add("A1", awayTeam + " to Win by 1 Goal");
            team.Add("A2+", awayTeam + " to Win by 2up Goals");
            team.Add("NG", "No Goal");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType412And429(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("0", "0 Goals");
            team.Add("1", "1 Goal");
            team.Add("2", "2 Goals");
            team.Add("3-OVER", "3 Goals &Over");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType442To444(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("1H", "First Half");
            team.Add("2H", "Second Half");
            team.Add("TIE", "Tie");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetType445To447(string betTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("YY", "Yes/Yes");
            team.Add("YN", "Yes/No");
            team.Add("NY", "No/Yes");
            team.Add("NN", "No/No");

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }

        private string GetStringBetTypeYesNo(string betTeam)
        {
            return betTeam == "Y" ? "Yes" : "No";
        }

        private string GetStringBetTypeOddEven(string betTeam)
        {
            return betTeam == "O" ? "Odd" : "Even";
        }

        private string GetStringBetTypeDoubleChance(string betTeam, string homeTeam, string awayTeam)
        {
            var team = new Dictionary<string, string>();
            team.Add("HD", homeTeam + "/" + "Draw");
            team.Add("HA", homeTeam + "/" + awayTeam);
            team.Add("H", homeTeam);
            team.Add("A", awayTeam);

            return team.ContainsKey(betTeam) == true ? team[betTeam] : string.Empty;
        }
    }
}
