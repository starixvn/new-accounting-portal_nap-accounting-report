namespace Fanex.AccountingReports.LegendManagement.Services.Impls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Data.Repository;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Models;
    using Fanex.AccountingReports.LegendManagement.Repositories.Impls;
    using Fanex.AccountingReports.LegendManagement.Resources;
    using Fanex.Common;

    public class LegendManagementService : ServiceBase
    {
        private const string InlineFileDataSuffix = ".inline";
        private MenuService _menuService;
        private LegendRepository _reportRepository;
        private readonly IDynamicRepository dynamicRepository = new DynamicRepository();

        public LegendManagementService()
        {
            this._menuService = new MenuService();
            this._reportRepository = new LegendRepository();
        }

        
        public List<MenuItem> GetMenuList(GetSystemMenuCriteria criteria)
        {
            return this._menuService.GetMenuList(criteria);
        }

        
        public string SaveReportLegend(SaveReportLegendCriteria criteria)
        {
            criteria.LegendId = this.MappingLegendId(new GetReportLegendCriteria
            {
                MenuId = criteria.MenuId,
                System = WebSettings.System
            });

            Hashtable outputParams = this._reportRepository.SaveReportLegend(criteria);
            return Converter.ToString2(outputParams[Labels.SP_Contants_Error]);
        }

        
        public LegendResultModel GetReportLegend(GetReportLegendCriteria criteria)
        {
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

        
        public string GetJsonpReportLegend(GetReportLegendCriteria criteria)
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

            return string.Empty;
        }

        
        public string UploadImage(UploadFileCriteria criteria)
        {
            var fileName = criteria.FileName.Substring(criteria.FileName.LastIndexOf('\\') + 1);
            var savePath = criteria.SavePath + "/" + fileName;
            criteria.File.SaveAs(savePath);
            var inlineData = MakeInlineImage(criteria.File.InputStream);
            this.SaveInlineData(inlineData, savePath + InlineFileDataSuffix);
            return criteria.FileName;
        }

        
        public List<string> MakeInlineImages(List<string> imagesPath, string imageFolder)
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

                    var base64String = this.GetInlineData(imagePath, imageFolder);
                    inlineImages.Add(base64String);
                }

                return inlineImages;
            }

            return null;
        }

        
        public string SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria)
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

        
        public List<InputedLegend> GetInputedLegendReport(GetInputedLegendReportCriteria criteria)
        {
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

        private int MappingLegendId(GetReportLegendCriteria criteria)
        {
            var legend = dynamicRepository.Get<dynamic>(new GetReportLegendCriteria
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
            if (criteria.System == WebSettings.System)
            {
                return (new Fanex.AccountingReports.Core.Services.Impl.MenuService()).GetMenuById(criteria.MenuId).Policy >= 1;
            }

            return true;
        }
    }
}
