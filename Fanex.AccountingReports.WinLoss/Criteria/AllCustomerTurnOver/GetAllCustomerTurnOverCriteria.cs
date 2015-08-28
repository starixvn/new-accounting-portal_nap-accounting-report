// -----------------------------------------------------------------------
// <copyright file="GetAllCustomerTurnOverCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllCustomerTurnOverCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Site { get; set; }

        public string From { get; set; }

        public bool IsPagingCallBack { get; set; }

        public int StartIndex { get; set; }

        public int UsersPerPage { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_AllCustomerTurnOver_GetReport";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}
