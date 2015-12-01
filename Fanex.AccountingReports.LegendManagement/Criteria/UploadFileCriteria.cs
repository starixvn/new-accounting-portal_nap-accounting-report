// --------------------------------------------------------------------------
// <copyright file="UploadFileCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using Fanex.Data.Repository;

    /// <summary>
    /// Upload File Criteria.
    /// </summary>
    public class UploadFileCriteria : CriteriaBase
    {
        public HttpPostedFileBase File { get; set; }

        public string FileName { get; set; }

        public string SavePath { get; set; }

        /// <summary>
        /// Check data's constrains.
        /// </summary>
        /// <returns>True: valid; False: invalid.</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}