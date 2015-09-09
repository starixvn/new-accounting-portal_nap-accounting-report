// --------------------------------------------------------------------------
// <copyright file="GetVirtualSportWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.VirtualSportWinLoss
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetVirtualSportWinLossCriteria : CriteriaBase, ICriteria
    {
        public string NormalLicensee { get; set; }

        public string Operator { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SportList { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_VirtualSportWinloss_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}