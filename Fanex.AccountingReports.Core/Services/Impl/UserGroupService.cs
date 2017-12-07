namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserGroupService : IUserGroupService
    {
        public UserGroup GetAdminGroup(GetAdminGroupCriteria criteria)
        {
            Hashtable outputParam = null;
            UserGroup userGroup = new UserGroup();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            userGroup.GroupId = Convert.ToInt32(outputParam["groupid"]);
            userGroup.GroupName = Convert.ToString(outputParam["groupname"]);

            return userGroup;
        }
    }
}
