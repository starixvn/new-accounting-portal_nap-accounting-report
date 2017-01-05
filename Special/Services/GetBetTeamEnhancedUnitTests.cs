using Fanex.AccountingReports.Special.Checkbet;
using Fanex.AccountingReports.UnitTests.Module.Special.TestUtility;
using NUnit.Framework;

namespace Fanex.AccountingReports.UnitTests.Module.Special.Services
{
    [TestFixture]
    public class GetBetTeamEnhancedUnitTests
    {
        [TestCase(401, "O", "Over 2.5")]
        [TestCase(402, "U", "Under 2.5")]
        [TestCase(405, "2:0", "2:0")]
        [TestCase(406, "G3", "3 Goals")]
        [TestCase(407, "G3", "3 Goals &Over")]
        [TestCase(408, "H2", "Barcelona to Win by 2 Goals")]
        [TestCase(410, "1X", "Barcelona or Draw")]
        [TestCase(411, "H", "Barcelona")]
        [TestCase(415, "HH", "Barcelona/Barcelona")]
        [TestCase(417, "YH", "Yes/Barcelona")]
        [TestCase(418, "YO", "Yes & Over 2.5")]
        [TestCase(419, "1H", "1st Half")]
        [TestCase(422, "H", "Home")]
        [TestCase(424, "OG", "Own Goal")]
        [TestCase(426, "H2+", "Barcelona to Win by 2up Goals")]
        [TestCase(427, "N", "No")]
        [TestCase(428, "o", "Odd")]
        [TestCase(429, "3-Over", "3 Goals &Over")]
        [TestCase(431, "HA", "Barcelona/Real")]
        [TestCase(443, "2H", "Second Half")]
        [TestCase(446, "YY", "Yes/Yes")]
        public void ContentChoiceBuilder_Return_True(int betType, string betTeam, string expect)
        {
            var service = new ContentChoiceBuilder();
            var result = service.GetContentBetchoiceToShowOf("Barcelona", "Real", betType, betTeam, 2.5m, 0);
            ComparisonAssert.AreEqual(result, expect);
        }
    }
}
