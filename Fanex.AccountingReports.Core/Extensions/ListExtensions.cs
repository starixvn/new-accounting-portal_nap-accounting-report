namespace Fanex.AccountingReports.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Common;

    public static class ListExtensions
    {
        public static void SortByProperty<T>(this List<T> dataSource, string sortType, string sortCol)
        {
            PropertyInfo propInfo = typeof(T).GetProperty(sortCol);
            if (propInfo == null || dataSource == null || dataSource.Count == 0)
            {
                return;
            }

            bool isAsc = string.Equals(sortType, SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
            Comparison<T> compare = delegate(T a, T b)
            {
                object valueA = isAsc
                    ? Converter.ToDecimal(propInfo.GetValue(a, null))
                    : Converter.ToDecimal(propInfo.GetValue(b, null));

                object valueB = isAsc
                    ? Converter.ToDecimal(propInfo.GetValue(b, null))
                    : Converter.ToDecimal(propInfo.GetValue(a, null));

                return valueA is IComparable ? ((IComparable)valueA).CompareTo(valueB) : 0;
            };

            dataSource.Sort(compare);
        }

        public static void SortDataByProperty<T>(List<T> dataSource, string sortDirection, string sortColumnName, string sortDataType)
        {
            PropertyInfo propInfo = typeof(T).GetProperty(sortColumnName);

            Comparison<T> compare = delegate(T a, T b)
            {
                bool isAsc = string.Equals(sortDirection, SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);
                object valueA;
                object valueB;

                switch (sortDataType)
                {
                    case SortDataTypeConstants.StringType:
                        valueA = isAsc ? propInfo.GetValue(a, null) : propInfo.GetValue(b, null);
                        valueB = isAsc ? propInfo.GetValue(b, null) : propInfo.GetValue(a, null);
                        break;
                    case SortDataTypeConstants.NumberType:
                        valueA = isAsc ?
                            Converter.ToDecimal(propInfo.GetValue(a, null)) :
                            Converter.ToDecimal(propInfo.GetValue(b, null));
                        valueB = isAsc ?
                            Converter.ToDecimal(propInfo.GetValue(b, null)) :
                            Converter.ToDecimal(propInfo.GetValue(a, null));
                        break;
                    case SortDataTypeConstants.DateType:
                        valueA = isAsc ?
                            Converter.ToDateTime(propInfo.GetValue(a, null)) :
                            Converter.ToDateTime(propInfo.GetValue(b, null));
                        valueB = isAsc ?
                            Converter.ToDateTime(propInfo.GetValue(b, null)) :
                            Converter.ToDateTime(propInfo.GetValue(a, null));
                        break;
                    default:
                        valueA = isAsc ? propInfo.GetValue(a, null) : propInfo.GetValue(b, null);
                        valueB = isAsc ? propInfo.GetValue(b, null) : propInfo.GetValue(a, null);
                        break;
                }

                return valueA is IComparable ? ((IComparable)valueA).CompareTo(valueB) : 0;
            };
            dataSource.Sort(compare);
        }
    }
}
