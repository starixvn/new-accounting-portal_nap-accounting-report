namespace Fanex.AccountingReports.Core.Extensions
{
    using System.IO;
    using System.Web;
    using Fanex.Excel;

    /// <summary>
    /// Work book Extensions.
    /// </summary>
    public static class WorkbookExtensions
    {
        public static void ResponseStreamExcel2(this FanexExcel workbook, string fileName)
        {
            var response = HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", fileName));
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            response.BinaryWrite(ms.ToArray());
        }
    }
}
