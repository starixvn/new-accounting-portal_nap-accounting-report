// -----------------------------------------------------------------------
// <copyright file="NumberGameFinancialRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Number Game Financial Report Repository.
    /// </summary>
    public class NumberGameFinancialRepository : INumberGameFinancialRepository
    {
        /// <summary>
        /// Gets the number game financial report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Get list data table of report.</returns>
        public DataSet GetNumberGameFinancial(GetNumberGameFinancialCriteria criteria)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                result = db.ExecuteDataSet(param);
            }

            return result;
        }

        /// <summary>
        /// Gets the number game financial report detail.
        /// </summary>
        /// <param name="ds">The data set.</param>
        /// <returns>Get list data detail.</returns>
        public List<NumberGameFinancial> GetNumberGameFinancialDetail(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return null;
            }

            var numberGameFinancialTable = ds.Tables[0];
            var numberGameFinancialDetail = new List<NumberGameFinancial>();

            foreach (DataRow row in numberGameFinancialTable.Rows)
            {
                numberGameFinancialDetail.Add(new NumberGameFinancial()
                {
                    ReportDate = DateTimeExtensions.ToShortFormatedDate(Converter.ToDateTime(row["rptdate"])),
                    Bettype = CacheRef.GetBettype(row["bettype"]),
                    IsLive = Converter.ToBool(row["isLive"]),
                    NumberPlayer = Converter.ToInt(row["NoOfPlayers"]),
                    NumberGame = Converter.ToInt(row["NoOfGames"]),
                    TotalBet = Convert.ToDecimal(row["TotalBets"]),
                    TotalWin = Convert.ToDecimal(row["TotalWins"])
                });
            }

            return numberGameFinancialDetail;
        }

        /// <summary>
        /// Gets the live number player and game report.
        /// </summary>
        /// <param name="ds">The data set.</param>
        /// <param name="grandTotal">The total.</param>
        /// <param name="reportDate">The report date.</param>
        /// <param name="isDateTotalGrand">The date total.</param>
        /// <param name="isLive">The live total.</param>
        /// <returns>The number player and game.</returns>
        public NumberGameFinancial GetLiveNumberPlayerAndGameReport(
                                                                    DataSet ds,
                                                                    NumberGameFinancial grandTotal,
                                                                    DateTime reportDate,
                                                                    bool isDateTotalGrand,
                                                                    bool isLive)
        {
            DataTable numberGameFinancialTable = null;

            if (isDateTotalGrand)
            {
                numberGameFinancialTable = ds.Tables[3];
            }
            else
            {
                if (isLive)
                {
                    numberGameFinancialTable = ds.Tables[2];
                }
                else
                {
                    numberGameFinancialTable = ds.Tables[1];
                }
            }

            var query = numberGameFinancialTable.AsEnumerable().First(obj => obj.Field<DateTime>("rptdate") == reportDate);

            grandTotal.NumberPlayer = query.Field<int>("NoOfPlayers");
            grandTotal.NumberGame = query.Field<int>("NoOfGames");

            return grandTotal;
        }

        public NumberGameFinancial GetGrandTotalNumberPlayerAndGameReport(DataSet ds, NumberGameFinancial grandTotal)
        {
            DataTable numberGameFinancialTable = ds.Tables[4];

            var query = numberGameFinancialTable.AsEnumerable().First();

            grandTotal.NumberPlayer = query.Field<int>("NoOfPlayers");
            grandTotal.NumberGame = query.Field<int>("NoOfGames");

            return grandTotal;
        }
    }
}