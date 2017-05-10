// --------------------------------------------------------------------------
// <copyright file="BetListRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.10">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Repositories
{
    using Fanex.BetList.Accounting.Core;
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.BetList.Core.Entities;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public interface BetListRepository
    {
        CombinationParlay BetList_SelDetailCombinationMP(ICriteria criteria);
        IList<S> Fetch<S>(dynamic criteria, System.Func<DataRow, S> mapObjectFunction) where S : class;
        IList<S> Fetch<S>(dynamic criteria, System.Func<DataRow, S> mapObjectFunction, ref List<ITicketData> outputList) where S : class;
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListRepositoryImp : BetListRepository
    {
        /// <summary>
        /// Fetches the specified data.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <returns>The specified data.</returns>
        public IList<S> Fetch<S>(dynamic criteria, Func<DataRow, S> mapObjectFunction) where S : class
        {
            IList<S> listData = new List<S>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria);
                if (dataSet != null
                    && dataSet.Tables.Count > 0)
                {
                    DataTable currentTable = dataSet.Tables[0];
                    foreach (DataRow row in currentTable.Rows)
                    {
                        listData.Add(mapObjectFunction(row));
                    }
                }
            }

            return listData;
        }

        /// <summary>
        /// Fetches the specified data.
        /// </summary>
        /// <typeparam name="S">The parameter type.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="mapObjectFunction">The map object function.</param>
        /// <param name="outputList">The output list.</param>
        /// <returns>The specified data.</returns>
        public IList<S> Fetch<S>(dynamic criteria, Func<DataRow, S> mapObjectFunction, ref List<ITicketData> outputList) where S : class
        {
            IList<S> listData = new List<S>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria);
                if (dataSet != null
                    && dataSet.Tables.Count > 0)
                {
                    DataTable currentTable = dataSet.Tables[0];
                    foreach (DataRow row in currentTable.Rows)
                    {
                        listData.Add(mapObjectFunction(row));
                    }

                    if (dataSet.Tables.Count > 1)
                    {
                        DataTable secondTable = dataSet.Tables[1];
                        foreach (DataRow row in secondTable.Rows)
                        {
                            outputList.Add(new TicketData
                            {
                                RefNo = Converter.ToString2(row["refno"]),
                                BetTypeId = Converter.To<int>(row["bettype"]),
                                Status = Converter.ToString2(row["status"]),
                                Odds = Converter.To<decimal>(row["odds"]),
                                Stake = Converter.To<decimal>(row["stake"]),
                                BetTeam = Converter.ToString2(row["betteam"]),
                                TransDesc = Converter.ToString2(row["TransDesc"])
                            });
                        }
                    }
                }
            }

            return listData;
        }

        /// <summary>
        /// Bets the list select detail combination mix parlay.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list select detail combination mix parlay.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public CombinationParlay BetList_SelDetailCombinationMP(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                DataSet dataSet = db.ExecuteDataSet(criteria.GetSpParams());
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTableMaster = dataSet.Tables[0];
                    DataTable dataTableDetail = dataSet.Tables[1];

                    CombinationParlay result = new CombinationParlay
                    {
                        GroupParlayEvents = new List<GroupParlayEvent>(),
                        Total = new ParlayEvent()
                    };

                    var winlostGroups = dataTableMaster.Rows.Cast<DataRow>()
                        .GroupBy(d => d["winlostdate"]);
                    foreach (var winlostGroup in winlostGroups)
                    {
                        GroupParlayEvent groupParlayEvent = new GroupParlayEvent
                        {
                            WinLossDate = Converter.ToDateTime(winlostGroup.Key),
                            ParlayEvents = new List<ParlayEvent>(),
                            SubTotal = new ParlayEvent()
                        };

                        foreach (var item in winlostGroup)
                        {
                            decimal winlost = Converter.ToDecimal(item["winlost"]);
                            decimal playercomm = Converter.ToDecimal(item["playercomm"]);

                            // calculate subtotal and total
                            groupParlayEvent.SubTotal.WinLoss += winlost + playercomm;
                            result.Total.WinLoss += winlost + playercomm;

                            // WIN:  serial != 0
                            // LOSE: serial == 0
                            int serial = Converter.ToInt(item["serial"]);

                            ParlayEvent parlayEvent = new ParlayEvent
                            {
                                Odds = Converter.ToDecimal(item["modds"]),
                                Stake = Converter.ToDecimal(item["stake"]),
                                WinLoss = winlost,
                                Comm = playercomm,
                                Status = Converter.ToString2(item["status"]).ToUpper()
                            };
                            groupParlayEvent.ParlayEvents.Add(parlayEvent);

                            // get Home -vs- Away
                            if (serial == 0)
                            {
                                // LOSE
                                parlayEvent.MatchList.Add(new Match
                                {
                                    Event = Labels.Total_of_losing_tickets
                                });
                            }
                            else
                            {
                                DataRow[] drowDetails = dataTableDetail.Select("serial = " + serial);
                                if (drowDetails != null)
                                {
                                    Match match;
                                    foreach (var dr in drowDetails)
                                    {
                                        match = new Match
                                        {
                                            HomeID = Converter.ToInt(dr["homeid"]),
                                            AwayID = Converter.ToInt(dr["awayid"])
                                        };
                                        string homeName = CacheRef.GetTeamName(match.HomeID);
                                        string awayName = CacheRef.GetTeamName(match.AwayID);
                                        match.Event = homeName + " -vs- " + awayName;

                                        parlayEvent.MatchList.Add(match);
                                    }
                                }
                            }
                        }

                        result.GroupParlayEvents.Add(groupParlayEvent);
                    }

                    return result;
                }
            }

            return null;
        }
    }
}