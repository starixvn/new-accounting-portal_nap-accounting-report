// -----------------------------------------------------------------------
// <copyright file="BetListStatement.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListStatement
    {
        public BetListStatement()
        {
            Language = "en";
            TransId = 0;
        }

        [SpParam(SqlParamName = "CustID")]
        public int CustId { get; set; }

        public int TransType { get; set; }

        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "language")]
        public string Language { get; set; }

        [SpParam(SqlParamName = "transid")]
        public int TransId { get; set; }

        public string GetSettingKey()
        {
            return "BetListByDayAll";
        }
    }
}