namespace Fanex.AccountingReports.Misc.Services.Implement.DuplicateTickets
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Misc.Models.DuplicateTickets;
    using Fanex.Common;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class DuplicateTicketsUtility
    {
        public const int TotalMilliseconds = 1000;

        
        public List<TicketInfo> AssignSubTicketInfo(IGrouping<dynamic, dynamic> duplicateTicketByUserName)
        {
            List<TicketInfo> groups = new List<TicketInfo>();
            DateTime prevBetTimeGroup = new DateTime(2014, 7, 11);
            DateTime prevBetTimeTicket = new DateTime(2014, 7, 11);
            int ticketIndex = 0;
            double timeDifferenceGroup = 0;
            double timeDifferenceTicket = 0;
            int groupNo = 0;

            foreach (var duplicateTicket in duplicateTicketByUserName)
            {
                DateTime betTime = duplicateTicket.bettime;
                TimeSpan timeSpanGroup = betTime.Subtract(prevBetTimeGroup);
                TimeSpan timeSpanTicket = betTime.Subtract(prevBetTimeTicket);
                timeDifferenceGroup = timeSpanGroup.TotalMilliseconds;
                timeDifferenceTicket = timeSpanTicket.TotalMilliseconds;

                if (ticketIndex == 0)
                {
                    timeDifferenceGroup = 0;
                    timeDifferenceTicket = 0;
                }

                if (timeDifferenceGroup < 0)
                {
                    continue;
                }

                if (timeDifferenceGroup >= TotalMilliseconds)
                {
                    groupNo++;
                    ticketIndex = 0;
                    timeDifferenceTicket = 0;
                }

                TicketInfo ticketInfo = new TicketInfo()
                {
                    Status = Converter.ToString2(duplicateTicket.STATUS),
                    ReferenceNumber = Converter.ToString2(duplicateTicket.refno),
                    BetTime = betTime.ToLongMMMddFormatedDate(),
                    TimeDifference = timeDifferenceTicket,
                    GroupNo = groupNo,
                    TransDesc = duplicateTicket.transdesc,
                    Site = Converter.ToString2(duplicateTicket.sitename),
                    BetFrom = Converter.ToString2(duplicateTicket.betfrom)
                };

                if (ticketIndex == 0)
                {
                    prevBetTimeGroup = betTime;
                }

                prevBetTimeTicket = betTime;

                ticketIndex++;
                groups.Add(ticketInfo);
            }

            return groups;
        }

        public DuplicateTicketsByUserName AssignMainTicketInfo(IGrouping<dynamic, dynamic> duplicateTicketByUserName)
        {
            DateTime winlostDate = Converter.ToDateTime(duplicateTicketByUserName.Key.winlostdate);
            var ticket = new DuplicateTicketsByUserName
            {
                UserName = Converter.ToString2(duplicateTicketByUserName.Key.username),
                CustId = Converter.ToInt(duplicateTicketByUserName.Key.custid),
                BetType = Converter.ToString2(duplicateTicketByUserName.Key.typename),
                WinLostDate = winlostDate.ToShortFormatedDate(),
                BalanceBeforeBet = Converter.ToDecimal(duplicateTicketByUserName.Key.balance),
                Stake = Converter.ToDecimal(duplicateTicketByUserName.Key.stake),
                Odds = Converter.ToDecimal(duplicateTicketByUserName.Key.odds),
                BetTypeId = Converter.To<int>(duplicateTicketByUserName.Key.bettype)
            };

            return ticket;
        }

        public string GetMD5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder builder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return builder.ToString();
        }
    }
}
