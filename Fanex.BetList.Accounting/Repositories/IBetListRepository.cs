// --------------------------------------------------------------------------
// <copyright file="IBetListRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.10">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Core.Entities;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IBetListRepository
    {
        IList<S> FetchMemberWinLoss<S>(ICriteria criteria, Func<DataRow, S> mapObjectFunction, ref List<ITicketData> outputList) where S : class;

        IList<TicketDetail> Fetch(dynamic criteria, CurrencyContants baseCurrency);

        IList<S> Fetch<S>(dynamic criteria, Func<DataRow, S> mapObjectFunction, ref Hashtable outputParams) where S : class;

        IList<S> Fetch<S>(dynamic criteria, Func<DataRow, S> mapObjectFunction) where S : class;

        IList<S> Fetch<S>(dynamic criteria, Func<DataRow, S> mapObjectFunction, ref List<ITicketData> outputList) where S : class;

        SystemParlayModel FetchSystemParlay(BetListSystemParlayByRefnoCriteria criteria);

        CombinationParlay BetList_SelDetailCombinationMP(ICriteria criteria);
    }
}