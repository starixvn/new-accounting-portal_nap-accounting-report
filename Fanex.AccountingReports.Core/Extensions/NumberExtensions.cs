namespace Fanex.AccountingReports.Core.Extensions
{
    using System;

    public static class NumberExtensions
    {
        public static string Format(this decimal value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        public static string Format(this object value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        public static string Format(this double value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        private static string DoFormat(object value, byte decDigits)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            string formatString = "{0:n" + decDigits.ToString() + "}";
            return string.Format(formatString, value);
        }
    }
}
