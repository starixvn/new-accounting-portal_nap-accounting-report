// -----------------------------------------------------------------------
// <copyright file="CreditTermExcelUtil.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using Fanex.AccountingReports.Main.Resources;

    /// <summary>
    /// The utilities methods used to export excel in Credit Term report.
    /// </summary>
    public class CreditTermExcelUtil
    {
        public static readonly string InstallmentAmountPrefixSessionKey =
            "Fanex.AccountingReports.Main.CreditTerm.InstallmentAmount.";

        /// <summary>
        /// Get new format date time.
        /// </summary>
        /// <param name="accountRemarkDate">The date of remark.</param>
        /// <returns>The new format.</returns>
        public static string GetDateTime(string accountRemarkDate)
        {
            string formatRemarkDate = string.Empty;

            var date = DateTime.Parse(accountRemarkDate);
            formatRemarkDate = date.Month + "/" + date.Day + " "
                               + date.Hour + ":" + date.Minute;

            return formatRemarkDate;
        }

        /// <summary>
        /// Get role name by identity.
        /// </summary>
        /// <param name="roleID">The role identity.</param>
        /// <returns>The name of role.</returns>
        public static string GetRoleNameByID(int roleID)
        {
            string roleName = string.Empty;

            if (roleID == 1)
            {
                roleName = Labels.Member;
            }
            else if (roleID == 2)
            {
                roleName = Labels.Agent;
            }
            else if (roleID == 3)
            {
                roleName = Labels.Master;
            }
            else if (roleID == 4)
            {
                roleName = Labels.Super;
            }

            return roleName;
        }
    }
}
