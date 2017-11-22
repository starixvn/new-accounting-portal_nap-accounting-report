namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Net;
    using Fanex.AccountingReports.Special.Criteria.SaveBetReport;
    using Fanex.AccountingReports.Special.Models.SaveBetReport;

    public interface ISaveBetReportService
    {
        Sport GetSports(GetSportsCriteria criteria);

        League GetLeagues(GetLeaguesCriteria criteria);

        Match GetMatchs(GetMatchsCriteria criteria);

        string GetFilePath(DownloadReportCriteria criteria);

        string GetFileName(DownloadReportCriteria criteria);

        string GetMimeType(string fileExtension);
    }
}
