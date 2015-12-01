// --------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Repositories.Impls
{
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// Base Repository.
    /// </summary>
    public class BaseRepository
    {
        /// <summary>
        /// Execute command to DB.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// Execute result.
        /// </returns>
        public virtual Hashtable ExecuteCommand(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria.GetSpParams());
                return db.GetOuputParams();
            }
        }
    }
}