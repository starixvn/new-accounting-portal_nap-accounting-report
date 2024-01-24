namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using Fanex.Logging;

    public enum LogType : int
    {
        Info = 1,

        Error = 2
    }

    public class LoggingHandler
    {
        public static void Log(Exception ex)
        {
            SqlException sqlException = ex as SqlException;
            var stackTrace = new StackTrace(ex);
            var methodname = stackTrace.GetFrames().Select(f => f.GetMethod()).FirstOrDefault();

            string message = ex.Message +
                              "<br />\r\nSOURCE: " + ex.Source +
                              "<br />\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                              "<br />\r\nREFERRER: " + HttpContext.Current.Request.UrlReferrer +
                              "<br />\r\nFORM: " + HttpContext.Current.Request.Form.ToString() +
                              "<br />\r\nQUERYSTRING: " + HttpContext.Current.Request.QueryString.ToString() +
                              (methodname != null ? "<br />\r\nMethodName: " + methodname.Name : string.Empty) +
                              "<br />\r\nTARGETSITE: " + ex.TargetSite +
                              "<br />\r\nSTACKTRACE: " + ex.StackTrace;
            if (sqlException != null)
            {
                message += "<br />\r\nProcedure:" + sqlException.Procedure +
                            "<br />\r\nLine Number: " + sqlException.LineNumber +
                            "<br />\r\nServer:" + sqlException.Server;
            }

            Logger.Log.Error(message);
        }

        public static void Log(string message = "", LogType logType = LogType.Error)
        {
            switch (logType)
            {
                case LogType.Info:
                    LogService.Info(message);
                    break;

                case LogType.Error:
                    LogService.Error(message);
                    break;
            }
        }
    }
}
