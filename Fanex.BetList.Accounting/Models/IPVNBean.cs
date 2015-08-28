// --------------------------------------------------------------------------
// <copyright file="IPVNBean.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.24">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class IPVNBean
    {
        public double IPFrom { get; set; }

        public double IPTo { get; set; }
    }
}