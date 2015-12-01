// --------------------------------------------------------------------------
// <copyright file="LoggingHandler.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.17">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Data.SqlClient;
    using System.Web;
    using Fanex.Logging;

    public enum LogType : int
    {
        /// <summary>
        /// The information type.
        /// </summary>
        Info = 1,

        /// <summary>
        /// The error type.
        /// </summary>
        Error = 2
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LoggingHandler
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void Log(Exception ex)
        {
            SqlException sqlException = ex as SqlException;
            string message = ex.Message +
                              "<br />\r\nSOURCE: " + ex.Source +
                              "<br />\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                              "<br />\r\nREFERRER: " + HttpContext.Current.Request.UrlReferrer +
                              "<br />\r\nFORM: " + HttpContext.Current.Request.Form.ToString() +
                              "<br />\r\nQUERYSTRING: " + HttpContext.Current.Request.QueryString.ToString() +
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

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logType">Type of the log.</param>
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