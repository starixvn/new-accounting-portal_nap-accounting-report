// -----------------------------------------------------------------------
// <copyright file="Sport.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sport model.
    /// </summary>
    [Serializable]
    public class Sport
    {
        public Sport()
        {
            Sports = new List<Sport>();
        }

        public int SportType { get; set; }

        public string SportName { get; set; }

        public List<Sport> Sports { get; set; }
    }
}