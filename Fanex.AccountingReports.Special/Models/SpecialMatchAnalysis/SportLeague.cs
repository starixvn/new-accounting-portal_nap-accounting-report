// --------------------------------------------------------------------------
// <copyright file="SportLeague.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportLeague : BaseModel
    {
        public SportLeague()
        {
            Leagues = new List<League>();
        }

        public List<League> Leagues { get; set; }
    }
}
