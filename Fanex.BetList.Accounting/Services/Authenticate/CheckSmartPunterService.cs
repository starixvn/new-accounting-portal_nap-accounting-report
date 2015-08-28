// -----------------------------------------------------------------------
// <copyright file="CheckSmartPunterService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Services
{
    using System.Collections;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.BetList.Accounting.Criteria.Authenticate;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckSmartPunterService : ICheckSmartPunterService
    {
        /// <summary>
        /// Check deny smart punter.
        /// </summary>
        /// <param name="custID">Customer Id.</param>
        /// <param name="userName">User name of customer.</param>
        /// <returns><c>True</c> If XXXX, <c>false</c> otherwise.</returns>
        public bool IsDeniedViewSmartPunter(int? custID, string userName)
        {
            if (PermissionService.HasReadPermission("SmartPunter"))
            {
                return false;
            }
            else
            {
                return this.CheckSmartPunter(new CheckSmartPunterCriteria
                {
                    CustID = custID.HasValue ? custID : null,
                    UserName = userName
                });
            }
        }

        /// <summary>
        /// Check smart punter service.
        /// </summary>
        /// <param name="criteria">Check smart punter criteria.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckSmartPunter(CheckSmartPunterCriteria criteria)
        {
            bool isSmartPunter = false;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                Hashtable hash = db.GetOuputParams();
                isSmartPunter = Converter.To<bool>(hash["issmartpunter"]);
            }

            return isSmartPunter;
        }
    }
}
