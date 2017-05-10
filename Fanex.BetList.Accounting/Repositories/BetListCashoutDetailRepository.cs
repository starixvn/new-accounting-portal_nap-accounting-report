using Fanex.BetList.Accounting.Models;
using Fanex.BetList.Core.Entities;
using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;

namespace Fanex.BetList.Accounting.Repositories
{
    public interface BetListCashoutDetailRepository
    {
        CashOutHistory GetCashOutHistory(ICriteria criteria);
    }

    public class BetListCashoutDetailRepositoryImp : BetListCashoutDetailRepository
    {
        public CashOutHistory GetCashOutHistory(ICriteria criteria)
        {
            TicketDetail ticketDetail = null;
            IList<CashOutTicket> cashOutTickets = new List<CashOutTicket>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader.Read())
                    {
                        ticketDetail = BuildTicketDetail(reader);
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            cashOutTickets.Add(BuildTicketData(reader));
                        }
                    }
                }
            }

            return new CashOutHistory
            {
                TicketDetail = ticketDetail,
                CashOutTickets = cashOutTickets
            };
        }

        private TicketDetail BuildTicketDetail(IDataReader reader, int custId = 0)
        {
            return new TicketDetail
            {
                Ticket = BuildCoreTicket(reader, custId),
                ChoiceHtml = string.Empty,
                OddHtml = string.Empty,
                StakeHtml = string.Empty,
                BetFrom = Convert.ToString(reader["betfrom"])
            };
        }

        private ITicket BuildCoreTicket(IDataReader reader, int custId = 0)
        {
            return new Ticket
            {
                CustId = custId,
                ActualStake = Convert.ToDecimal(reader["actual_stake"]),
                AgentWinlost = Convert.ToDecimal(reader["awinlost"]),
                AgentPositionTaking = Convert.ToDecimal(reader["apositiontaking"]),
                Comm = Convert.ToDecimal(reader["comm"]),
                AgentDiscount = Convert.ToDecimal(reader["discount"]),

                AwayId = Convert.ToInt64(reader["awayid"]),
                BetId = Convert.ToInt64(reader["betid"]),
                BetTeam = Convert.ToString(reader["betteam"]),
                BetTypeId = Convert.ToInt32(reader["bettype"]),
                CommStatus = Convert.ToString(reader["comstatus"]),
                Handicap1 = Convert.ToDecimal(reader["hdp1"]),
                Handicap2 = Convert.ToDecimal(reader["hdp2"]),
                HomeId = Convert.ToInt64(reader["homeid"]),
                EventDate = Convert.ToDateTime(reader["eventdate"]),
                IP = Convert.ToString(reader["ip"]),
                IsLive = Convert.ToBoolean(reader["liveindicator"]),
                IsNeutral = Convert.ToBoolean(reader["isNeutral"]),
                LeagueId = Convert.ToInt32(reader["leagueid"]),
                LiveHomeScore = Convert.ToInt32(reader["livehomescore"]),
                LiveAwayScore = Convert.ToInt32(reader["liveawayscore"]),

                MasterWinlost = Convert.ToDecimal(reader["mwinlost"]),
                MasterPositionTaking = Convert.ToDecimal(reader["mpositiontaking"]),
                AgentComm = Convert.ToDecimal(reader["acomm"]),
                MasterDiscount = Convert.ToDecimal(reader["adiscount"]),

                MasterOdds = Convert.ToDecimal(reader["modds"]),
                MatchCode = Convert.ToString(reader["matchcode"]),
                MatchId = Convert.ToInt32(reader["matchid"]),
                Odds = Convert.ToDecimal(reader["odds"]),
                OddsType = Convert.ToString(reader["OddsType"]),
                PlayerComm = Convert.ToDecimal(reader["commission"]),
                RefNo = Convert.ToString(reader["refno"]),
                ShowTime = Convert.ToString(reader["showtime"]),
                SportTypeId = Convert.ToInt32(reader["sporttype"]),
                Stake = Convert.ToDecimal(reader["stake"]),
                Status = Convert.ToString(reader["status"]),

                SuperWinlost = Convert.ToDecimal(reader["swinlost"]),
                SuperPositionTaking = Convert.ToDecimal(reader["spositiontaking"]),
                SuperComm = Convert.ToDecimal(reader["scomm"]),
                SuperDiscount = Convert.ToDecimal(reader["sdiscount"]),

                TransDate = Convert.ToDateTime(reader["transdate"]),
                TransId = Convert.ToInt64(reader["transid"]),
                Winlost = Convert.ToDecimal(reader["winlost"]),
                WinlostDate = Convert.ToDateTime(reader["winlostdate"]),
                TransDesc = Convert.ToString(reader["transdesc"]),

                BetCheck = Convert.ToString(reader["betcheck"]),
                OddsSpread = Convert.ToDecimal(reader["OddsSpread"]),

                OriginalBetType = Convert.ToInt32(reader["originalbettype"]),
                BetdaqId = Convert.ToInt64(reader["betdaqid"]),
                ExtraInfo = Convert.ToString(reader["extrainfo"])
            };
        }

        private CashOutTicket BuildTicketData(IDataReader reader)
        {
            return new CashOutTicket
            {
                BetType = Convert.ToInt32(reader["bettype"]),
                TransId = Convert.ToInt64(reader["transid"]),
                RefNo = Convert.ToInt64(reader["refno"]),
                Status = Convert.ToString(reader["status"]),
                Stake = Convert.ToDecimal(reader["stake"]),
                TransDate = Convert.ToDateTime(reader["transdate"]),
                CustId = Convert.ToInt32(reader["custid"]),
                Price = Convert.ToDecimal(reader["price"]),
                WinLost = Convert.ToDecimal(reader["winlost"])
            };
        }
    }
}
