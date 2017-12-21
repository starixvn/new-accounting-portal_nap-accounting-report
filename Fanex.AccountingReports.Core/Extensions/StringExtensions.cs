namespace Fanex.AccountingReports.Core.Extensions
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class StringExtensions
    {
        private const string JSONPFormat = "{0}({{Reponse : \"{1}\"}});";

        /// <summary>
        /// To the string1.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <returns>The string value.</returns>
        public static string ToString1(this object value)
        {
            return value == null ?
                string.Empty :
                value.ToString();
        }

        public static string JsonpFormat(string callBackFunction, string reponse)
        {
            return string.Format(
                JSONPFormat,
                callBackFunction,
                reponse.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\"", "\\\""));
        }
    }
}