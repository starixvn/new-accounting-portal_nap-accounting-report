// -----------------------------------------------------------------------
// <copyright file="NumberOfActiveMembers.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Racing.Models.NumberOActiveMembersForRacing
{
    using System;

    /// <summary>
    /// Racing - Number Of Active Member Model.
    /// </summary>
    [Serializable]
    public class NumberOfActiveMembers
    {
        public int TotalActiveMembers { get; set; }
    }
}