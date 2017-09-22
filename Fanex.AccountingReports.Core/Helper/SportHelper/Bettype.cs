// --------------------------------------------------------------------------
// <copyright file="Bettype.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Helper.SportHelper
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.AccountingReports.Core.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetType
    {
        /// <summary>
        /// Gets the bet type.
        /// </summary>
        /// <param name="betTypeId">The bet type identifier.</param>
        /// <returns>The bet type.</returns>
        public static string GetBetType(int betTypeId)
        {
            string betypeName = CacheRef.GetBettype(betTypeId);
            if (IsVirtualSportBetType(betTypeId))
            {
                betypeName += Labels.Symbol_VirtualSportSuffix;
            }

            return betypeName;
        }

        private static bool IsVirtualSportBetType(int betTypeId)
        {
            return BetTypeConstants.MinVirtualSportBetType <= betTypeId &&
                betTypeId <= BetTypeConstants.MaxVirtualSportBetType;
        }

        private static bool IsSportingSolutionBetType(int betTypeId)
        {
            return BetTypeConstants.MinSportingSolutionBetType <= betTypeId &&
                betTypeId <= BetTypeConstants.MaxSportingSolutionBetType;
        }
    }
}
