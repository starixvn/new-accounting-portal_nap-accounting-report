// --------------------------------------------------------------------------
// <copyright file="ILegendManagementService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;

    public interface ILegendManagementService
    {
        List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria);

        string SaveReportLegend(SaveReportLegendCriteria criteria);

        LegendResultModel GetReportLegend(GetReportLegendCriteria criteria);

        string GetJsonpReportLegend(GetReportLegendCriteria criteria);

        string UploadImage(UploadFileCriteria criteria);

        List<string> MakeInlineImages(List<string> imagesPath, string imageFolder);

        string SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria);

        List<InputedLegend> GetInputedLegendReport(GetInputedLegendReportCriteria criteria);
    }
}
