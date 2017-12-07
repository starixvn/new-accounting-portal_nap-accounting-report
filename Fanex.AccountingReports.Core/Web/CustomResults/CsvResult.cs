namespace Fanex.AccountingReports.Core.Web.CustomResults
{
    using System;
    using System.IO;
    using ExcelUtil.Spreadsheet;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CsvResult : SpreadSheetResult
    {
        public CsvResult(ExcelFile file, string fileName)
            : base(file, fileName)
        {
        }

        protected override string ContentType
        {
            get { return "application/csv"; }
        }

        protected override string Extension
        {
            get { return ".csv"; }
        }

        protected override Action<ExcelFile, Stream> SaveAction
        {
            get { return (f, s) => f.SaveCsv(s, CsvType.CommaDelimited); }
        }
    }
}
