// -----------------------------------------------------------------------
// <copyright file="Hourly.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class Hourly
    {
        public Hourly()
        {
            Name = string.Empty;
            OnlinePlayers = "0";
            NumberOfGames = string.Empty;
            Currency = string.Empty;
            TotalBets = string.Empty;
            TotalWins = string.Empty;
            GameIncome = string.Empty;
            GamePayout = string.Empty;
            CompanyTO = string.Empty;
            CompanyComm = string.Empty;
            CompanyNetInCome = string.Empty;

            DataOnEachHourList = new List<Hourly>();
            DataAccumulatedList = new List<Hourly>();
        }

        public string Name { get; set; }

        public string OnlinePlayers { get; set; }

        public string NumberOfGames { get; set; }

        public string Currency { get; set; }

        public string TotalBets { get; set; }

        public string TotalWins { get; set; }

        public string GameIncome { get; set; }

        public string GamePayout { get; set; }

        public string CompanyTO { get; set; }

        public string CompanyComm { get; set; }

        public string CompanyNetInCome { get; set; }

        public List<Hourly> DataOnEachHourList { get; set; }

        public List<Hourly> DataAccumulatedList { get; set; }
    }
}