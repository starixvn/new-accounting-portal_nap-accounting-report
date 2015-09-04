// -----------------------------------------------------------------------
// <copyright file="IUserGroupService.cs" company="Nexcel Solutions Vietnam.">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Core.Services.Impl.Interface
{
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IUserGroupService
    {
        UserGroup GetAdminGroup(GetAdminGroupCriteria criteria);
    }
}