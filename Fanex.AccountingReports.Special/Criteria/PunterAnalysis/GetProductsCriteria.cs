// --------------------------------------------------------------------------
// <copyright file="GetProductsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.PunterAnalysis
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetProductsCriteria : CriteriaBase
    {
        public bool IsSB { get; set; }

        public override string GetSettingKey()
        {
            return "Special_PunterAnalysis_GetProducts";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}