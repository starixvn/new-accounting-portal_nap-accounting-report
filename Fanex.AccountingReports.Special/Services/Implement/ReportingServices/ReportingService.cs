namespace Fanex.AccountingReports.Special.Services.Implement.ReportingServices
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Criteria.ReportingServices;
    using Fanex.AccountingReports.Special.Models.ReportingServices;
    using Fanex.AccountingReports.Special.Services.Interface;

    /// <summary>
    /// Reporting service.
    /// </summary>
    public class ReportingService : ServiceBase, IReportingService
    {
        public string DefaultPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ReportingServicesPath"];
            }
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Get files and folders of report directory.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public Report GetReport(GetReportCriteria criteria)
        {
            try
            {
                var report = new Report();

                string defaultPathWithSlash = DefaultPath + "\\";
                string path = string.IsNullOrEmpty(criteria.Path) ? DefaultPath : defaultPathWithSlash + criteria.Path;

                if (Directory.Exists(path))
                {
                    var parentFolderIsDefaultPath = string.Equals(path, DefaultPath);
                    string parentFolderFullName = parentFolderIsDefaultPath ? string.Empty : Directory.GetParent(path).FullName;
                    bool isDefaultPath = string.Equals(parentFolderFullName, DefaultPath);
                    report.ParentFolder = isDefaultPath ? string.Empty : parentFolderFullName.Replace(defaultPathWithSlash, string.Empty);
                    report.ParentFolderName = parentFolderIsDefaultPath ? string.Empty : Directory.GetParent(path).Name;
                    string[] subdirectoryEntries = Directory.GetDirectories(path);
                    string[] files = Directory.GetFiles(path);

                    if (subdirectoryEntries.Length > 0)
                    {
                        foreach (var subdirectory in subdirectoryEntries)
                        {
                            var directoryInfo = new Report
                            {
                                Path = subdirectory.Replace(defaultPathWithSlash, string.Empty),
                                FolderName = new DirectoryInfo(subdirectory).Name
                            };

                            report.Folders.Add(directoryInfo);
                        }
                    }

                    if (files.Length > 0)
                    {
                        foreach (var file in files)
                        {
                            var fileInfo = new Report
                            {
                                Path = file.Replace(defaultPathWithSlash, string.Empty),
                                FileName = Path.GetFileName(file)
                            };

                            report.Files.Add(fileInfo);
                        }
                    }
                }

                return report;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>Mime type of file.</returns>
        /// <exception cref="System.ArgumentNullException">Extension is null.</exception>
        public string GetMimeType(string fileExtension)
        {
            if (fileExtension == null)
            {
                throw new ArgumentNullException("extension is null");
            }

            if (fileExtension.StartsWith("."))
            {
                fileExtension = fileExtension.Substring(1);
            }

            switch (fileExtension.ToLower())
            {
                case "xls":
                    return "application/vnd.ms-excel";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "doc":
                    return "application/msword";
                case "zip":
                    return "application/x-zip-compressed";
                case "pdf":
                    return "application/pdf";
                default:
                    return System.Net.Mime.MediaTypeNames.Application.Octet;
            }
        }
    }
}
