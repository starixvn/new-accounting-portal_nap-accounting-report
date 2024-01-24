namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using System.Web;
    using Fanex.Data.Repository;

    public class UploadFileCriteria : CriteriaBase
    {
        public HttpPostedFileBase File { get; set; }

        public string FileName { get; set; }

        public string SavePath { get; set; }

        public override string GetSettingKey()
        {
            return string.Empty;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
