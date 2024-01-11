namespace Fanex.AccountingReports.Main.Services.Implement.CreditTerm
{
    using System;
    using Fanex.AccountingReports.Main.Resources;

    public class CreditTermExcelUtil
    {
        public static readonly string InstallmentAmountPrefixSessionKey =
            "Fanex.AccountingReports.Main.CreditTerm.InstallmentAmount.";

        public static string GetDateTime(string accountRemarkDate)
        {
            string formatRemarkDate = string.Empty;

            var date = DateTime.Parse(accountRemarkDate);
            formatRemarkDate = date.Month + "/" + date.Day + " "
                               + date.Hour + ":" + date.Minute;

            return formatRemarkDate;
        }

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
