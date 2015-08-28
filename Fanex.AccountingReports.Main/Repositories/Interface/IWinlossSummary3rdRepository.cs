// --------------------------------------------------------------------------
// <copyright file="IWinlossSummary3rdRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.25">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.NAP.Framework.Data;

    public interface IWinlossSummary3rdRepository
    {
        void RecalculateWinLoss(ICriteria criteria);
    }
}
