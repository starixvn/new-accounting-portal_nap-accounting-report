// --------------------------------------------------------------------------
// <copyright file="IPeakConcurrentUserService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.16">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Services.Interface
{
    using Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IPeakConcurrentUserService
    {
        PeakConcurrentUser GetConcurrentUserList(PeakConcurrentUserCriteria peakConcurrentUserCriteria);

        void PeakExport2Excel(PeakConcurrentUserCriteria peakConcurrentUserCriteria);
    }
}
