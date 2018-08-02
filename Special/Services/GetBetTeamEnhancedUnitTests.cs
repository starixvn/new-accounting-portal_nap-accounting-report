using Fanex.AccountingReports.Special.Checkbet;
using NUnit.Framework;

namespace Fanex.AccountingReports.UnitTests.Module.Special.Services
{
    [TestFixture]
    public class GetBetTeamEnhancedUnitTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [TestCase(401, "O", ExpectedResult = "Over")]
        [TestCase(402, "U", ExpectedResult = "Under")]
        [TestCase(405, "2:0", ExpectedResult = "2:0")]
        [TestCase(406, "G3", ExpectedResult = "3 Goals")]
        [TestCase(407, "G3", ExpectedResult = "3+")]
        [TestCase(411, "H", ExpectedResult = "Barcelona")]
        [TestCase(417, "YH", ExpectedResult = "Yes/Home")]
        [TestCase(418, "YO", ExpectedResult = "Yes & Over")]
        [TestCase(419, "1H", ExpectedResult = "First Half")]
        [TestCase(422, "H", ExpectedResult = "Home")]
        [TestCase(424, "OG", ExpectedResult = "Own Goal")]
        [TestCase(427, "N", ExpectedResult = "No")]
        [TestCase(428, "o", ExpectedResult = "Odd")]
        [TestCase(429, "3-Over", ExpectedResult = "3+")]
        [TestCase(443, "2H", ExpectedResult = "Second Half")]
        [TestCase(446, "YY", ExpectedResult = "Yes/Yes")]
        public string ContentChoiceBuilder_Return_True(int betType, string betTeam)
        {
            var service = new ContentChoiceBuilder();
            return service.GetContentBetchoiceToShowOf("Barcelona", "Real", betType, betTeam, 2.5m, 0, 0.5m);
        }


        [TestCase(603, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(603, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(604, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(604, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(605, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(605, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(606, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(606, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(607, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(607, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(609, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(609, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(610, "O", ExpectedResult = "Over")]
        [TestCase(610, "U", ExpectedResult = "Under")]
        [TestCase(611, "O", ExpectedResult = "Odd")]
        [TestCase(611, "E", ExpectedResult = "Even")]
        [TestCase(612, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(612, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(613, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(613, "A", ExpectedResult = "Houston Rockets")]
        [TestCase(615, "O", ExpectedResult = "Over")]
        [TestCase(615, "U", ExpectedResult = "Under")]
        [TestCase(616, "O", ExpectedResult = "Over")]
        [TestCase(616, "U", ExpectedResult = "Under")]
        [TestCase(617, "H", ExpectedResult = "Cleveland Cavaliers")]
        [TestCase(617, "A", ExpectedResult = "Houston Rockets")]
        public string ContentChoiceBuilder_BAIO_ReturnsRightResult(int betType, string betTeam)
        {
            // Arrange
            // Initialize RefData. It always returns empty for first time
            var service = new ContentChoiceBuilder();
            return service.GetContentBetchoiceToShowOf("Cleveland Cavaliers", "Houston Rockets", betType, betTeam, 0, 0, 0.5m);
        }
    }
}
