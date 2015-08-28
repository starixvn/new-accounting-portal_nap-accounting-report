// --------------------------------------------------------------------------
// <copyright file="CalcOdds.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Helper.SportHelper
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CalcOdds
    {
        private static readonly List<int> BetTypes;

        static CalcOdds()
        {
            BetTypes = new List<int>() { 1, 2, 3, 7, 12, 8, 20, 124, 125 };

            for (int i = 81; i <= 88; i++)
            {
                BetTypes.Add(i);
            }

            for (int i = 91; i <= 120; i++)
            {
                BetTypes.Add(i);
            }
        }

        /// <summary>
        /// Gets the odds.
        /// </summary>
        /// <param name="odds">The odds value.</param>
        /// <param name="betType">The bet type.</param>
        /// <param name="oddsType">Type of the odds.</param>
        /// <returns>The odds result.</returns>
        public static decimal GetOdds(decimal odds, int betType, int oddsType)
        {
            decimal oddsValue = odds;
            if (BetTypes.Contains(betType) && oddsType == 1)
            {
                oddsValue = odds + 1;
            }
            else if (BetTypes.Contains(betType) && oddsType == 5)
            {
                oddsValue = odds * 100;
            }

            return oddsValue;
        }

        /// <summary>
        /// Gets the type of the odds.
        /// </summary>
        /// <param name="betType">The bet type.</param>
        /// <param name="oddsType">Type of the odds.</param>
        /// <returns>The type of the odds.</returns>
        public static int GetOddsType(int betType, int oddsType)
        {
            if (BetTypes.Contains(betType))
            {
                return oddsType;
            }

            return 0;
        }
    }
}