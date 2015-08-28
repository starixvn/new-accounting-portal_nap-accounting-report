// -----------------------------------------------------------------------
// <copyright file="ISaveBetReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Net;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Models.SaveBetReport;

    public interface ISaveBetReportService
    {
        NetworkCredential NetworkCredential { get; }

        string IP { get; }

        Sport GetSports(GetSportsCriteria criteria);

        League GetLeagues(GetLeaguesCriteria criteria);

        Match GetMatchs(GetMatchsCriteria criteria);

        string GetFilePath(DownloadReportCriteria criteria);

        string GetFileName(DownloadReportCriteria criteria);

        string GetMimeType(string fileExtension);
    }
}