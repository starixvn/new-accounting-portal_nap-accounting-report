// -----------------------------------------------------------------------
// <copyright file="ICompanyWinLossRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Repositories.Interface
{
    using System.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICompanyWinLossRepository
    {
        IDataReader GetDataFromDB(ICriteria criteria);        
    }
}
