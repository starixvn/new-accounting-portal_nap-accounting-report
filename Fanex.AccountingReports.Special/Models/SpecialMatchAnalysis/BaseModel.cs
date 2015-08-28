// --------------------------------------------------------------------------
// <copyright file="BaseModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>    
    [Serializable]
    public class BaseModel
    {
        public BaseModel()
        {
            Name = string.Empty;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
