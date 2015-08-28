// --------------------------------------------------------------------------
// <copyright file="GetLiveCasinoWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.LiveCasinoWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLiveCasinoWinLossCriteria : CriteriaBase, ICriteria
    {
        public string NormalLicensee { get; set; }

        public string Operator { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int GameType { get; set; }

        [SpParam(Ignored = true)]
        public string Title { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_LiveCasinoWinloss_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
