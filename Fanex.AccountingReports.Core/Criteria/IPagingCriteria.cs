// --------------------------------------------------------------------------
// <copyright file="IPagingCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Criteria
{
    using System.Diagnostics.CodeAnalysis;

    public interface IPagingCriteria
    {
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        int PageIndex { get; set; }

        // StartIndex = PageIndex * PageSize;
        // EndIndex = (PageIndex + 1) * PageSize - 1;       
    }
}
