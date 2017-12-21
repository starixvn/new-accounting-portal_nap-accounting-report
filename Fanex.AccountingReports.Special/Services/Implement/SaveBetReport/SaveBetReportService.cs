namespace Fanex.AccountingReports.Special.Services.Implement.SaveBetReport
{
    using System;
    using System.IO;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Models.SaveBetReport;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SaveBetReportService : ServiceBase, ISaveBetReportService
    {
        public SaveBetReportService()
        {
        }

        /// <summary>
        /// Gets the sports.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Sports data.</returns>
        public Sport GetSports(GetSportsCriteria criteria)
        {
            var result = new Sport();
            try
            {
                using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var _params = criteria.GetSpParams();
                    var reader = objectDb.ExecuteReader(_params);

                    while (reader.Read())
                    {
                        var sportInfo = new Sport
                        {
                            SportType = Converter.ToInt(reader["SportType"]),
                            SportName = Converter.ToString2(reader["SportName"])
                        };

                        result.Sports.Add(sportInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the leagues.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Leagues data.</returns>
        public League GetLeagues(GetLeaguesCriteria criteria)
        {
            var result = new League();

            try
            {
                using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var _params = criteria.GetSpParams();
                    var reader = objectDb.ExecuteReader(_params);

                    while (reader.Read())
                    {
                        var leagueInfo = new League
                        {
                            LeagueId = Converter.ToInt(reader["leagueid"]),
                            LeagueName = Converter.ToString2(reader["leaguename"])
                        };

                        result.Leagues.Add(leagueInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the match data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List match data.</returns>
        public Match GetMatchs(GetMatchsCriteria criteria)
        {
            var result = new Match();

            try
            {
                using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var _params = criteria.GetSpParams();
                    var reader = objectDb.ExecuteReader(_params);

                    while (reader.Read())
                    {
                        var matchInfo = new Match()
                        {
                            MatchId = Converter.ToInt(reader["MatchID"]),
                            MatchCode = Converter.ToString2(reader["MatchCode"]),
                            HomeTeam = Converter.ToString2(reader["Home"]),
                            AwayTeam = Converter.ToString2(reader["Away"]),
                            EventDate = Converter.ToDateTime(reader["EventDate"]).ToShortFormatedDateWithSlash().Replace("/", "_"),

                            SportType = Converter.ToInt(reader["SportType"])
                        };

                        result.Matchs.Add(matchInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Path of file.</returns>
        public string GetFilePath(DownloadReportCriteria criteria)
        {
            string filePath = string.Empty;
            string formatFileName = string.Empty;
            string parentDirectory = string.Empty;

            var eventDate = criteria.EventDate.Replace("_", "-");
            DateTime reportDate = DateTime.ParseExact(eventDate, "MM-dd-yyyy", null);
            string eventYear = reportDate.Year.ToString();
            string eventMonth = reportDate.ToString("MM");
            string eventDateStr = reportDate.ToString("dd");
            string reportFolderContainer = string.Join(string.Empty, new string[] { eventYear, eventMonth, eventDateStr });

            DateTime matchEventDate = Convert.ToDateTime(eventDate);
            var compareReportDate = DateTime.Compare(matchEventDate, AppSettings.NewSaveBetReportDate);

            if (compareReportDate >= 0)
            {
                string saveBetReportPath = AppSettings.SaveBetReportDataDirectory;
                parentDirectory = Path.Combine(saveBetReportPath, reportFolderContainer);
                formatFileName = string.Join(".", new string[] { criteria.ReportName, "xlsx" });
                filePath = Path.Combine(parentDirectory, formatFileName);
            }
            else
            {
                string oldSaveBetReportPath = AppSettings.OldSaveBetReportDataDirectory;
                parentDirectory = Path.Combine(oldSaveBetReportPath, reportFolderContainer);
                formatFileName = string.Join(".", new string[] { criteria.ReportName, "xls" });
                filePath = Path.Combine(parentDirectory, formatFileName);
            }

            return filePath;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The file name.</returns>
        public string GetFileName(DownloadReportCriteria criteria)
        {
            string fileName = string.Empty;
            var eventDate = criteria.EventDate.Replace("_", "-");
            DateTime matchEventDate = Convert.ToDateTime(eventDate);
            var compareReportDate = DateTime.Compare(matchEventDate, AppSettings.NewSaveBetReportDate);

            if (compareReportDate >= 0)
            {
                fileName = string.Join(".", new string[] { criteria.ReportName, "xlsx" });
            }
            else
            {
                fileName = string.Join(".", new string[] { criteria.ReportName, "xls" });
            }

            return fileName;
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