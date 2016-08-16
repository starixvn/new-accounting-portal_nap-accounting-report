using Fanex.AccountingReports.Special.Services.Implement;
using Fanex.AccountingReports.UnitTests.Module.Special.TestUtility;
using NUnit.Framework;

namespace Fanex.AccountingReports.UnitTests.Module.Special.Services
{
    [TestFixture]
    public class GetBetTeamEnhancedUnitTests
    {
        private string homeTeam = "Barcelona";
        private string awayTeam = "Real";
        private string betTeam = string.Empty;
        private int betType = 0;
        private decimal hdp1 = 2.5m;
        private GetBetTeamEnhanced service = new GetBetTeamEnhanced();

        [Test]
        public void GetStringBetTypeFrom401to404_Return_Over()
        {
            // Arrange
            var expect = "Over 2.5";
            betType = 401;
            betTeam = "O";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetTypeFrom401to404_Return_Under()
        {
            // Arrange
            var expect = "Under 2.5";
            betType = 402;
            betTeam = "U";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringFollowBetType_Return_BetTeam()
        {
            // Arrange
            var expect = "2:0";
            betType = 405;
            betTeam = "2:0";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType406_Return_True()
        {
            // Arrange
            var expect = "3 Goals";
            betType = 406;
            betTeam = "G3";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType407_Return_True()
        {
            // Arrange
            var expect = "3 Goals &Over";
            betType = 407;
            betTeam = "G3";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType408_Return_True()
        {
            // Arrange
            var expect = "Barcelona to Win by 2 Goals";
            betType = 408;
            betTeam = "H2";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType410_Return_True()
        {
            // Arrange
            var expect = "Barcelona or Draw";
            betType = 410;
            betTeam = "1X";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType411_Return_True()
        {
            // Arrange
            var expect = "Barcelona";
            betType = 411;
            betTeam = "H";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType415_Return_True()
        {
            // Arrange
            var expect = "Barcelona/Barcelona";
            betType = 415;
            betTeam = "HH";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType417_Return_True()
        {
            // Arrange
            var expect = "Yes/Barcelona";
            betType = 417;
            betTeam = "YH";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType418_Return_True()
        {
            // Arrange
            var expect = "Yes & Over 2.5";
            betType = 418;
            betTeam = "YO";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType419To421_Return_True()
        {
            // Arrange
            var expect = "1st Half";
            betType = 419;
            betTeam = "1H";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType422To423_Return_True()
        {
            // Arrange
            var expect = "Home";
            betType = 422;
            betTeam = "H";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType424_Return_True()
        {
            // Arrange
            var expect = "Own Goal";
            betType = 424;
            betTeam = "OG";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType426_Return_True()
        {
            // Arrange
            var expect = "Barcelona to Win by 2up Goals";
            betType = 426;
            betTeam = "H2+";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType412And429_Return_True()
        {
            // Arrange
            var expect = "3 Goals &Over";
            betType = 429;
            betTeam = "3-Over";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType442To444_Return_True()
        {
            // Arrange
            var expect = "Second Half";
            betType = 443;
            betTeam = "2H";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetType445To447_Return_True()
        {
            // Arrange
            var expect = "Yes/Yes";
            betType = 446;
            betTeam = "YY";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetTypeYesNo_Return_True()
        {
            // Arrange
            var expect = "No";
            betType = 427;
            betTeam = "N";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetTypeOddEven_Return_True()
        {
            // Arrange
            var expect = "Odd";
            betType = 428;
            betTeam = "o";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }

        [Test]
        public void GetStringBetTypeDoubleChance_Return_True()
        {
            // Arrange
            var expect = "Barcelona/Real";
            betType = 431;
            betTeam = "HA";

            // Atc
            var result = service.GetStringFollowBetType(homeTeam, awayTeam, betType, betTeam, hdp1);

            // Assert
            ComparisonAssert.AreEqual(result, expect);
        }
    }
}
