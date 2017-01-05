// --------------------------------------------------------------------------
// <copyright file="LegendManagementService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Services.Impls
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.AccountingReports.LegendManagement.Constants;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;
    using Fanex.AccountingReports.LegendManagement.Repositories.Impls;
    using Fanex.AccountingReports.LegendManagement.Repositories.Interfaces;
    using Fanex.AccountingReports.LegendManagement.Resources;
    using Fanex.AccountingReports.LegendManagement.Services.Interfaces;
    using Fanex.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Legend Management Service.
    /// </summary>
    public class LegendManagementService : ServiceBase, ILegendManagementService
    {
        private const string InlineFileDataSuffix = ".inline";
        private IMenuService _menuService;
        private ILegendRepository _reportRepository;

        public LegendManagementService() :
            this(null, null)
        {
        }

        public LegendManagementService(IMenuService menuService, ILegendRepository reportRepository)
        {
            this._menuService = menuService ?? new MenuService();
            this._reportRepository = reportRepository ?? new LegendRepository();
        }

        /// <summary>
        /// Gets the menu list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The menu list.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria)
        {
            try
            {
                return this._menuService.GetMenuList(criteria);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Saves the report legend.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Save result.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public string SaveReportLegend(SaveReportLegendCriteria criteria)
        {
            try
            {
                criteria.LegendId = this.MappingLegendId(new GetReportLegendCriteria
                {
                    MenuId = criteria.MenuId,
                    System = WebSettings.System
                });

                Hashtable outputParams = this._reportRepository.SaveReportLegend(criteria);
                return Converter.ToString2(outputParams[Labels.SP_Contants_Error]);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return Labels.Error;
        }

        /// <summary>
        /// Gets the report legend.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report legend.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public LegendResultModel GetReportLegend(GetReportLegendCriteria criteria)
        {
            try
            {
                // return Labels.SampleTemplate;
                var rawData = GetReportData<dynamic>(criteria).FirstOrDefault();
                if (rawData != null)
                {
                    var reportLegendContent = rawData.Legend;
                    return new LegendResultModel
                    {
                        Content = reportLegendContent,
                        IsDefault = false
                    };
                }

                criteria.LegendId = this.MappingLegendId(new GetReportLegendCriteria());
                rawData = GetReportData<dynamic>(criteria).FirstOrDefault();

                return new LegendResultModel
                {
                    Content = rawData != null ? rawData.Legend : Labels.SampleTemplate,
                    IsDefault = true
                };
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new LegendResultModel
                {
                    Content = string.Empty,
                    IsDefault = false
                };
            }
        }

        /// <summary>
        /// Gets the JSONP report legend.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The JSONP report legend.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public string GetJsonpReportLegend(GetReportLegendCriteria criteria)
        {
            try
            {
                if (!HasPermission(criteria))
                {
                    return string.Empty;
                }

                var rawData = GetReportData<dynamic>(criteria).FirstOrDefault();
                if (rawData != null)
                {
                    var reportLegendContent = rawData.Legend;
                    return reportLegendContent;
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>File name.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public string UploadImage(UploadFileCriteria criteria)
        {
            try
            {
                var fileName = criteria.FileName.Substring(criteria.FileName.LastIndexOf('\\') + 1);
                var savePath = criteria.SavePath + "/" + fileName;
                criteria.File.SaveAs(savePath);
                var inlineData = MakeInlineImage(criteria.File.InputStream);
                this.SaveInlineData(inlineData, savePath + InlineFileDataSuffix);
                return criteria.FileName;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Makes the inline images.
        /// </summary>
        /// <param name="imagesPath">The images path.</param>
        /// <param name="imageFolder">The image folder.</param>
        /// <returns>The inline images.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<string> MakeInlineImages(List<string> imagesPath, string imageFolder)
        {
            try
            {
                if (imagesPath != null)
                {
                    var inlineImages = new List<string>();
                    var imageLength = imagesPath.Count;
                    for (int i = 0; i < imageLength; i++)
                    {
                        var imagePath = imagesPath[i];
                        if (imagePath.Contains("data:image"))
                        {
                            continue;
                        }

                        // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imagePath);
                        // HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        // Stream stream = response.GetResponseStream();
                        // var base64String = this.MakeInlineImage(stream);
                        // response.Close();
                        // inlineImages.Add(base64String);
                        var base64String = this.GetInlineData(imagePath, imageFolder);
                        inlineImages.Add(base64String);
                    }

                    return inlineImages;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Saves the report legend mapping.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Save result.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public string SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria)
        {
            try
            {
                criteria.LegendId = this.MappingLegendId(new GetReportLegendCriteria
                {
                    MenuId = criteria.AccMenuId,
                    System = WebSettings.System
                });

                if (criteria.LegendId != 0)
                {
                    Hashtable outputParams = this._reportRepository.SaveReportLegendMapping(criteria);
                    return Converter.ToString2(outputParams[Labels.SP_Contants_Error]);
                }

                return Labels.NotExistLegend;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return Labels.Error;
        }

        /// <summary>
        /// Gets the inputted legend report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The inputted legend report.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<InputedLegend> GetInputedLegendReport(GetInputedLegendReportCriteria criteria)
        {
            try
            {
                // var rawData = GetReportData<InputedLegend>(criteria);
                // var result = rawData.ToList();
                List<InputedLegend> result = new List<InputedLegend>();
                result.Add(new InputedLegend
                {
                    System = "Accounting",
                    ReportId = 101,
                    ReportName = "Daily Win Loss 2h",
                    Content = "abc"
                });

                result.Add(new InputedLegend
                {
                    System = "NAP",
                    ReportId = 101,
                    ReportName = "Daily Win Loss 2h",
                    Content = "abc"
                });

                result.Add(new InputedLegend
                {
                    System = "NAP",
                    ReportId = 103,
                    ReportName = "Bet Count",
                    Content = "Bet Count"
                });

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return null;
        }

        /// <summary>
        /// Mappings the legend identifier.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The legend identifier.</returns>
        private int MappingLegendId(GetReportLegendCriteria criteria)
        {
            var legend = Repository.Get<dynamic>(new GetReportLegendCriteria
            {
                MenuId = criteria.MenuId,
                System = WebSettings.System
            });

            if (legend == null)
            {
                return 0;
            }

            return Converter.ToInt(legend.LegendID);
        }

        /// <summary>
        /// Makes the inline image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The inline image.</returns>
        private string MakeInlineImage(Stream stream)
        {
            var base64String = string.Empty;
            using (BinaryReader br = new BinaryReader(stream))
            {
                byte[] buffer = new byte[16 * 5000];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    var content = ms.ToArray();
                    base64String = Convert.ToBase64String(content, 0, content.Length);
                }
            }

            return base64String;
        }

        private void SaveInlineData(string inlineData, string filePath)
        {
            using (var file = new StreamWriter(filePath))
            {
                file.WriteLine(inlineData);
                file.Flush();
            }
        }

        /// <summary>
        /// Gets the inline data.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <param name="imageFolder">The image folder.</param>
        /// <returns>The inline data.</returns>
        private string GetInlineData(string imagePath, string imageFolder)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(imagePath))
            {
                var filePath = imageFolder + "\\" + imagePath.Substring(imagePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1) + InlineFileDataSuffix;
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }

            return result;
        }

        private bool HasPermission(GetReportLegendCriteria criteria)
        {
            switch (criteria.System)
            {
                case SystemName.AccNapReport:
                    return (new Fanex.AccountingReports.Core.Services.Impl.MenuService()).GetMenuById(criteria.MenuId).Policy >= 1;
                default:
                    return true;
            }
        }
    }
}
