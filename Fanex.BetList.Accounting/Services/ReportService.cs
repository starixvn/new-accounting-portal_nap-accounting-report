namespace Fanex.BetList.Accounting.Services
{
    using Fanex.BetList.Accounting.Models;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ReportService
    {
        public const string BetTrans14SPSuffix = "_14";
        public const string WarehouseSPSuffix = "_WH";

        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                BalanceUp balanceUp = null;

                using (ObjectDb objDb = new ObjectDb("Core_ReportService_GetBalanceUpDay"))
                {
                    balanceUp = objDb.QueryEntity<BalanceUp>();
                }

                return balanceUp;
            }
        }

        #endregion StaticProperties
    }
}