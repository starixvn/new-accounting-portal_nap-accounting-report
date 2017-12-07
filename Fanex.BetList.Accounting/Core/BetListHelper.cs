namespace Fanex.BetList.Accounting.Core
{
    using BetList.Core.Builder;
    using BetList.Core.Entities;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Resources;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListHelper
    {
        public static readonly Regex IsNumericRegex = new Regex("^(" +
            /*Hex*/ @"0x[0-9a-f]+" + "|" +
            /*Bin*/ @"0b[01]+" + "|" +
            /*Oct*/ @"0[0-7]+" + "|" +
            /*Dec*/ @"((?!0)|[-+])(\d*\.)?\d+(e\d+)?" +
                                                    ")$");

        public static bool IsNumeric(string value)
        {
            return IsNumericRegex.IsMatch(value);
        }

        public string IsNegative(decimal input)
        {
            return input < 0 ? "negative" : "positive";
        }

        public bool IsHRWinPlaceBettype(int betType)
        {
            return betType == BetTypeContants.WinPlace ||
                betType == BetTypeContants.WinPlaceUKTote ||
                betType == BetTypeContants.VirtualSportWinPlace;
        }

        public bool IsHRFixOdds(int betType)
        {
            return betType == BetTypeContants.WinUKTote ||
              betType == BetTypeContants.PlaceUKTote ||
              betType == BetTypeContants.WinPlaceUKTote;
        }

        public bool IsHRBettype(int betType)
        {
            return betType == BetTypeContants.Win ||
                betType == BetTypeContants.Place ||
                betType == BetTypeContants.WinPlace ||
                betType == BetTypeContants.WinUKTote ||
                betType == BetTypeContants.PlaceUKTote ||
                betType == BetTypeContants.WinPlaceUKTote;
        }

        public bool IsVirtualHRBettype(int betType)
        {
            return betType == BetTypeContants.VirtualSportWin ||
                betType == BetTypeContants.VirtualSportPlace ||
                betType == BetTypeContants.VirtualSportWinPlace;
        }

        public string PercentFormat(decimal input)
        {
            var isInputInterger = (input * 100) % 1 == 0;
            return isInputInterger ?
                Math.Round(input * 100, 0).ToString() + Labels.Symbol_Percent :
                Math.Round(input * 100, 2).ToString() + Labels.Symbol_Percent;
        }

        public string DecorateCashOutHistoryLink(string status, TicketDetail betDetail, ITicketHelper ticketHelper)
        {
            var cashOutHistoryLink = new BetListHTMLBuilder().CreateStatusBuilder(betDetail.Ticket.BetTypeId).Render(betDetail.Ticket, ticketHelper, new List<ITicketData>(), true)
                .CashOutHistory
                .ToString();
            return status + cashOutHistoryLink;
        }
    }
}
