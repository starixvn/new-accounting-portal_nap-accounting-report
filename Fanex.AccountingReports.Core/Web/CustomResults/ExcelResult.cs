// --------------------------------------------------------------------------
// <copyright file="ExcelResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Web.CustomResults
{
    using System;
    using System.IO;
    using ExcelUtil.Spreadsheet;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExcelResult : SpreadSheetResult
    {
        public ExcelResult(ExcelFile file, string fileName)
            : base(file, fileName)
        {
        }

        protected override string ContentType
        {
            get { return "application/vnd.ms-excel"; }
        }

        protected override string Extension
        {
            get { return ".xls"; }
        }

        protected override Action<ExcelFile, Stream> SaveAction
        {
            get { return (f, s) => f.SaveXls(s); }
        }
    }
}
