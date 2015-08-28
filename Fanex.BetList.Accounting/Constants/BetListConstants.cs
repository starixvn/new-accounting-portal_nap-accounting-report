// --------------------------------------------------------------------------
// <copyright file="BetListConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Constants
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetListConstants
    {
        public const string ScoreInformationHomeTeamId = "1";
        public const string ScoreInformationAwayTeamId = "2";

        public const string ScoreInformationRefund = "Refund";
        public const string ScoreInformationRefundHT = "Refund HT";
        public const string ScoreInformationRefundFT = "Refund FT";

        public const int ScoreInformationSetsOfBasketBall = 5; // 4sets, OT
        public const int ScoreInformationSetsOfHockeySport = 4; // 3sets, OT
        public const int ScoreInformationSetsOfTennis = 5; // 5sets
        public const int ScoreInformationSetsOfBaseball = 9; // 9sets
        public const int ScoreInformationSetsOfBadminton = 5; // 5sets

        public const string BetListByCustomerInfoSessionKey = "Fanex.AccountingReports.BetList.Contants.BetListByCustomerInfoSessionKey";
        public const string BetListOutputLiveCasinoSessionKey = "Fanex.AccountingReports.BetList.Contants.BetListOutputLiveCasinoSessionKey";
        public const string BetListByUsernameSessionKey = "Fanex.AccountingReports.BetList.Contants.BetListByUsernameSessionKey";
        public const string BetListByStatementSessionKey = "Fanex.AccountingReports.BetList.Contants.BetListByStatementSessionKey";
        public const string BetListByStatementOutputLiveCasinoSessionKey = "Fanex.AccountingReports.BetList.Contants.BetListByStatementOutputLiveCasinoSessionKey";
    }
}