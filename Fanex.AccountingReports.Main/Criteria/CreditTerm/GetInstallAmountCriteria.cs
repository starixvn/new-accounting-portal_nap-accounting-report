// -----------------------------------------------------------------------
// <copyright file="GetInstallAmountCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Criteria.CreditTerm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Install Amount report.
    /// </summary>
    public class GetInstallAmountCriteria : CriteriaBase
    {
        public int RoleID { get; set; }

        public override string GetSettingKey()
        {
            return "InstallAmount";
        }

        public override bool IsValid()
        {
            return RoleID == 1 || RoleID == 2 || RoleID == 3 || RoleID == 4;
        }
    }
}
