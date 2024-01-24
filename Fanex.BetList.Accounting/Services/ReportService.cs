namespace Fanex.BetList.Accounting.Services
{
    using Fanex.BetList.Accounting.Models;
    using Fanex.Data;

    public static class ReportService
    {
        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                BalanceUp balanceUp = null;

                using (var objDb = ObjectDbFactory.CreateInstance("Core_ReportService_GetBalanceUpDay"))
                {
                    balanceUp = objDb.QueryEntity<BalanceUp>();
                }

                return balanceUp;
            }
        }

        #endregion StaticProperties
    }
}
