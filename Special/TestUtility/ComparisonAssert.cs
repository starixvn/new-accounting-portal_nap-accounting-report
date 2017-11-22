using Newtonsoft.Json;
using NUnit.Framework;

namespace Fanex.AccountingReports.UnitTests.Module.Special.TestUtility
{
    internal class ComparisonAssert
    {
        internal static void AreEqual(object expected, object actual)
        {
            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
