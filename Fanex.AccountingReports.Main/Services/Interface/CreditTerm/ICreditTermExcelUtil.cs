namespace Fanex.AccountingReports.Main.Services.Interface.CreditTerm
{
    /// <summary>
    /// The interface of utilities method used to export excel in Credit Term report.
    /// </summary>
    public interface ICreditTermExcelUtil
    {
        string GetDateTime(string accountRemarkDate);

        string GetRoleNameByID(int roleID);
    }
}
