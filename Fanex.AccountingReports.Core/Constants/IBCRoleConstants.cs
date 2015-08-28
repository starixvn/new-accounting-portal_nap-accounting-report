// -----------------------------------------------------------------------
// <copyright file="IBCRoleConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.09.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// IBC role constant.
    /// </summary>
    public static class IBCRoleConstants
    {
        public const int Super = 4;
        public const int Master = 3;
        public const int Agent = 2;
        public const int Member = 1;
        public const int Company = 0;
    }
}
