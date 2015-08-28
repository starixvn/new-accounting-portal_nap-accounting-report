// --------------------------------------------------------------------------
// <copyright file="ExternalDailyWinlostService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Test.Services.Implement
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Test.Criteria;
    using Fanex.AccountingReports.Test.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalDailyWinlostService : ServiceBase
    {
        /// <summary>
        /// Gets the daily win loss.
        /// </summary>
        /// <returns>The daily win loss.</returns>
        public ExternalDailyWinlostViewModel GetDailyWinLoss()
        {
            var dailyWinlostCriteria = new ExternalDailyWinlossCriteria();

            dailyWinlostCriteria.Winlostdate = Convert.ToDateTime("01/07/2013");
            dailyWinlostCriteria.Custid = null;
            dailyWinlostCriteria.GroupName = "ACCINDO";

            var rawData = GetReportData<dynamic>(dailyWinlostCriteria);
            var dailyWinLost = new ExternalDailyWinlostViewModel();

            if (rawData != null)
            {
                foreach (var data in rawData)
                {
                    var winlostDetail = new ExternalDailyWinlostDetail();
                    winlostDetail.CustId = data.CustID;
                    winlostDetail.Username = Convert.ToString(data.username);
                    winlostDetail.TotalTo = Convert.ToString(data.totalTO);
                    winlostDetail.CompanyTo = Convert.ToString(data.ComTO);
                    winlostDetail.SuperTo = Convert.ToString(data.SAgTO);
                    winlostDetail.MasterTo = Convert.ToString(data.MAgTO);
                    winlostDetail.AgentTo = Convert.ToString(data.AgTO);
                    winlostDetail.CompanyWinlost = Convert.ToString(data.ComWinlost);
                    winlostDetail.SuperWinlost = Convert.ToString(data.SAgWinlost);
                    winlostDetail.MasterWinlost = Convert.ToString(data.MAgWinlost);
                    winlostDetail.AgentWinlost = Convert.ToString(data.AgWinlost);

                    dailyWinLost.AddToList(winlostDetail);
                }
            }

            return dailyWinLost;
        }
    }
}