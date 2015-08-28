// --------------------------------------------------------------------------
// <copyright file="INumberGameWinLossByBetTypeMatchService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    public interface INumberGameWinLossByBetTypeMatchService
    {
        List<EntityBase<int, string>> GetCurrencyList();
    }
}
