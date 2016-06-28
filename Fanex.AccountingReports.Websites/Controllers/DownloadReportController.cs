// --------------------------------------------------------------------------
// <copyright file="DownloadReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Websites.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.Common;
    using Fanex.ServiceManagement.Utility;

    /// <summary>
    /// Download report controller.
    /// </summary>
    public class DownloadReportController : Controller
    {
        private string _ip = AppSettings.IP;
        private string _password = AppSettings.Password;
        private string _username = AppSettings.UserName;

        private NetworkCredential _networkCredential;

        public DownloadReportController()
        {
            if (AppSettings.IsAuthenticate)
            {
                if (AppSettings.IsEncryptPassword)
                {
                    _password = Cryptography.CfsDeCrypt(_password);
                }

                _networkCredential = new NetworkCredential(_username, _password);
            }
        }

        public ActionResult DownloadReport(string time, string reportName)
        {
            DateTime currentTime = DateTime.Now;

            string parentReportPath = AppSettings.SMAReportDataDirectory;
            string reportTimeString = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
            DateTime reportTime = Convert.ToDateTime(reportTimeString);
            DateTime nextMonthReportTime = reportTime.AddMonths(1);
            string reportContainerFolder = reportTime.Year.ToString();

            string reportNameString = Cryptography.CfsDeCrypt(reportName.Replace(" ", "+"));

            if (AppSettings.IsAuthenticate)
            {
                using (new NetworkConnection(@"\\" + _ip, _networkCredential))
                {
                    return GetExcelFile(currentTime, parentReportPath, reportTime, nextMonthReportTime, reportContainerFolder, reportNameString);
                }
            }
            else
            {
                return GetExcelFile(currentTime, parentReportPath, reportTime, nextMonthReportTime, reportContainerFolder, reportNameString);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private ActionResult GetExcelFile(DateTime currentTime, string parentReportPath, DateTime reportTime, DateTime nextMonthReportTime, string reportContainerFolder, string reportNameWithExtension)
        {
            string reportPath = Path.Combine(new string[] { parentReportPath, reportContainerFolder, reportNameWithExtension });
            if (!System.IO.File.Exists(reportPath))
            {
                return RedirectToAction(
                    "ExpiredNotification",
                    "Common",
                    new { type = 1, area = "UserLogin" });
            }

            TimeSpan duration = currentTime - reportTime;
            TimeSpan expiredDuration = nextMonthReportTime - reportTime;

            if (duration.TotalDays < expiredDuration.TotalDays)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(reportPath);
                string fileExtension = Path.GetExtension(reportPath);
                var mimeType = "application/x-zip-compressed";
                var contentResult = File(fileBytes, mimeType, reportNameWithExtension);
                return contentResult;
            }
            else
            {
                return RedirectToAction(
                    "ExpiredNotification",
                    "Common",
                    new { message = 2, area = "UserLogin" });
            }
        }
    }
}