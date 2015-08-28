// --------------------------------------------------------------------------
// <copyright file="ILegendRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Repositories.Interfaces
{
    using System.Collections;
    using Fanex.AccountingReports.LegendManagement.Criteria;

    public interface ILegendRepository
    {
        Hashtable SaveReportLegend(SaveReportLegendCriteria criteria);

        Hashtable SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria);
    }
}
