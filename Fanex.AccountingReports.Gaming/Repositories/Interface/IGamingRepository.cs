namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IGamingRepository
    {
        IDataReader GetLicenseesByProduct(GetLicenseesByProductCriteria criteria);
    }
}
