namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections;
    using System.Data;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;

    public class NumberGameStatisticRepository : INumberGameStatisticRepository
    {
        public NumberGameStatisticByBettype GetNumberGameStatisticByBettype(GetNumberGameStatisticCriteria criteria)
        {
            var report = new NumberGameStatisticByBettype()
            {
                GrandTotal = new NumberGameStatisticByBettypeDetail()
            };

            Hashtable outputParams = new Hashtable();
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                var reportParams = criteria.GetSpParams();
                var reader = objectDb.ExecuteReader(reportParams);
                while (reader.Read())
                {
                    var item = new NumberGameStatisticByBettypeDetail
                    {
                        Bettype = Converter.ToString2(reader["Bettype"]),
                        Players = Converter.ToInt(reader["Players"]),
                        BetCount = Converter.ToInt(reader["BetCount"]),
                        TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                        TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                        GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                        GamePayout = Converter.ToDecimal(reader["GamePayout"]) / 100,
                        CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                        NetIncome = Converter.ToDecimal(reader["NetIncome"])
                    };

                    this.CalculateTotal(report.GrandTotal, item);
                    report.ReportRows.Add(item);
                }

                reader.Close();
                outputParams = objectDb.GetOuputParams();
                report.GrandTotal.Players = Converter.ToInt(outputParams["totalplayers"]);
                CalCulateTotalGamePayout(report.GrandTotal);
            }

            return report;
        }

        public NumberGameStatisticByDaily GetNumberGameStatisticByDaily(GetNumberGameStatisticCriteria criteria)
        {
            var report = new NumberGameStatisticByDaily()
            {
                GrandTotal = new NumberGameStatisticByDailyDetail()
            };

            Hashtable outputParams = new Hashtable();
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                var reportParams = criteria.GetSpParams();
                var reader = objectDb.ExecuteReader(reportParams);
                while (reader.Read())
                {
                    var item = new NumberGameStatisticByDailyDetail
                    {
                        Date = Converter.ToDateTime(reader["Date"]).ToShortFormatedDate(),
                        Players = Converter.ToInt(reader["Players"]),
                        BetCount = Converter.ToInt(reader["BetCount"]),
                        TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                        TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                        GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                        GamePayout = Converter.ToDecimal(reader["GamePayout"]) / 100,
                        CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                        NetIncome = Converter.ToDecimal(reader["NetIncome"])
                    };

                    this.CalculateTotal(report.GrandTotal, item);
                    report.ReportRows.Add(item);
                }

                reader.Close();
                outputParams = objectDb.GetOuputParams();
                report.GrandTotal.Players = Converter.ToInt(outputParams["totalplayers"]);
                CalCulateTotalGamePayout(report.GrandTotal);
            }

            return report;
        }

        public NumberGameStatisticByLeague GetNumberGameStatisticByLeague(GetNumberGameStatisticCriteria criteria)
        {
            var report = new NumberGameStatisticByLeague()
            {
                GrandTotal = new NumberGameStatisticByLeagueDetail()
            };

            Hashtable outputParams = new Hashtable();
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                var reportParams = criteria.GetSpParams();
                var reader = objectDb.ExecuteReader(reportParams);
                while (reader.Read())
                {
                    var item = new NumberGameStatisticByLeagueDetail
                    {
                        LeagueName = Converter.ToString2(reader["LeagueName"]),
                        Players = Converter.ToInt(reader["Players"]),
                        BetCount = Converter.ToInt(reader["BetCount"]),
                        TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                        TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                        GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                        GamePayout = Converter.ToDecimal(reader["GamePayout"]) / 100,
                        CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                        NetIncome = Converter.ToDecimal(reader["NetIncome"])
                    };

                    this.CalculateTotal(report.GrandTotal, item);
                    report.ReportRows.Add(item);
                }

                reader.Close();
                outputParams = objectDb.GetOuputParams();
                report.GrandTotal.Players = Converter.ToInt(outputParams["totalplayers"]);
                CalCulateTotalGamePayout(report.GrandTotal);
            }

            return report;
        }

        public NumberGameStatisticByUserName GetNumberGameStatisticByUserName(GetNumberGameStatisticCriteria criteria)
        {
            var report = new NumberGameStatisticByUserName()
            {
                GrandTotal = new NumberGameStatisticByUserNameDetail()
            };

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
                var reportParams = criteria.GetSpParams();
                var reader = objectDb.ExecuteReader(reportParams);
                while (reader.Read())
                {
                    var item = new NumberGameStatisticByUserNameDetail
                    {
                        UserName = Converter.ToString2(reader["UserName"]),
                        CustId = Converter.ToInt(reader["CustId"]),
                        BetCount = Converter.ToInt(reader["BetCount"]),
                        TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                        TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                        GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                        GamePayout = Converter.ToDecimal(reader["GamePayout"]) / 100,
                        CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                        NetIncome = Converter.ToDecimal(reader["NetIncome"])
                    };

                    this.CalculateTotal(report.GrandTotal, item);
                    report.ReportRows.Add(item);
                }

                reader.Close();
            }

            CalCulateTotalGamePayout(report.GrandTotal);
            return report;
        }

        private void CalculateTotal(NumberGameStatisticDetail total, NumberGameStatisticDetail item)
        {
            total.BetCount += item.BetCount;
            total.TotalBets += item.TotalBets;
            total.TotalWins += item.TotalWins;
            total.GameIncome += item.GameIncome;
            total.CompanyTurnover += item.CompanyTurnover;
            total.NetIncome += item.NetIncome;
        }

        private void CalCulateTotalGamePayout(NumberGameStatisticDetail total)
        {
            total.GamePayout = total.TotalBets == 0 ? 0 : total.TotalWins / total.TotalBets;
        }

        private NumberGameStatisticDetail CreateNumberGameStatisticDetail(IDataReader reader)
        {
            return new NumberGameStatisticDetail()
            {
                BetCount = Converter.ToInt(reader["BetCount"]),
                TotalBets = Converter.ToDecimal(reader["TotalBets"]),
                TotalWins = Converter.ToDecimal(reader["TotalWins"]),
                GameIncome = Converter.ToDecimal(reader["GameIncome"]),
                GamePayout = Converter.ToDecimal(reader["GamePayout"]) / 100,
                CompanyTurnover = Converter.ToDecimal(reader["CompanyTurnover"]),
                NetIncome = Converter.ToDecimal(reader["NetIncome"])
            };
        }
    }
}