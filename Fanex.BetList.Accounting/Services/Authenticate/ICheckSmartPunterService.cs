// -----------------------------------------------------------------------
// <copyright file="ICheckSmartPunterService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Services
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICheckSmartPunterService
    {
        /// <summary>
        /// Check punter is smart or not.
        /// </summary>
        /// <param name="custID">Customer identify.</param>
        /// <param name="userName">Customer username.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool IsDeniedViewSmartPunter(int? custID, string userName);
    }
}
