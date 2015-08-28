// -----------------------------------------------------------------------
// <copyright file="CheckSmartPunterCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria.Authenticate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckSmartPunterCriteria : CriteriaBase
    {
        public int? CustID { get; set; }

        public string UserName { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelCheckSmartPunter";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
