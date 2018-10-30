namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameWinLossByBetTypeMatch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByBetTypeMatchService : ServiceBase
    {
        public static void ExportColumnMargin(
            decimal turnover,
            decimal margin,
            ref ExcelCell currentCell)
        {
            if (turnover != 0)
            {
                currentCell.Value = margin;
            }
            else
            {
                currentCell.Value = Labels.NotAvailable;
            }
        }

        /// <summary>
        /// Sets the odd even color export excel.
        /// </summary>
        /// <param name="textStyle">The text style.</param>
        /// <param name="twoDecimalNumberStyle">The two decimal number style.</param>
        /// <param name="twoDecimalPositiveNumberStyle">The two decimal positive number style.</param>
        /// <param name="twoDecimalPositivePercentNumberStyle">The two decimal positive percent number style.</param>
        /// <param name="isEvenRow">If set to <c>true</c> [is even row].</param>
        public static void SetOddEvenColor_ExportExcel(
            ref CellStyle textStyle,
            ref CellStyle twoDecimalNumberStyle,
            ref CellStyle twoDecimalPositiveNumberStyle,
            ref CellStyle twoDecimalPositivePercentNumberStyle,
            bool isEvenRow)
        {
            if (isEvenRow)
            {
                var evenRowColor = ExcelExtensions.EvenRowColor;
                textStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalPositiveNumberStyle.FillPattern.SetSolid(evenRowColor);
                twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(evenRowColor);
            }
            else
            {
                var oddRowColor = ExcelExtensions.OddRowColor;
                textStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalNumberStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalPositiveNumberStyle.FillPattern.SetSolid(oddRowColor);
                twoDecimalPositivePercentNumberStyle.FillPattern.SetSolid(oddRowColor);
            }
        }

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <returns>The currency list.</returns>
        public List<EntityBase<int, string>> GetCurrencyList()
        {
            try
            {
                var result = new List<EntityBase<int, string>>();
                var currencyList = ReportService.ExchangeCurrencies;
                if (currencyList == null)
                {
                    return result;
                }

                foreach (var row in currencyList)
                {
                    if (ReportService.IsExcludeCurrency(row.Value))
                    {
                        continue;
                    }

                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.Key,
                        Name = row.Value
                    });
                }

                result = result.OrderBy(info => info.Name).ToList();
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}