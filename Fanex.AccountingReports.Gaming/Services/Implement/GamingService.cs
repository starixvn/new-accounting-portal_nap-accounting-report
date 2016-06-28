// --------------------------------------------------------------------------
// <copyright file="GamingService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Models;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GamingService : ServiceBase, IGamingService
    {
        private IGamingRepository _gamingRepository = null;

        public GamingService()
        {
            _gamingRepository = new GamingRepository();
        }

        /// <summary>
        /// Gets the licensee support bingo.
        /// </summary>
        /// <returns>The licensee support bingo.</returns>
        public List<EntityBase<int, string>> GetLicensee_SupportBingo()
        {
            try
            {
                var licenseeList = new List<EntityBase<int, string>>();
                EntityBase<int, string> info = new EntityBase<int, string>
                {
                    Id = CompanyConstants.IBC,
                    Name = Labels.IBC
                };

                licenseeList.Add(info);
                var rawData = ReportService.GetLicensee_SupportByProduct(new GetLicensee_SupportByProductCriteria
                {
                    ProductPrefix = ProductPrefixConstants.Bingo
                });

                if (rawData != null)
                {
                    foreach (var data in rawData)
                    {
                        info = new EntityBase<int, string>
                        {
                            Id = data.Id,
                            Name = data.Name
                        };

                        licenseeList.Add(info);
                    }
                }

                return licenseeList;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the company list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The company list.</returns>
        public CompanyInfo GetNumberGameCompanyList(GetNumberGameCompanyListCriteria criteria)
        {
            try
            {
                var result = new CompanyInfo
                {
                    ChildInfo = new List<CompanyInfo>()
                };

                CompanyInfo info = new CompanyInfo
                {
                    Id = CompanyConstants.IBC,
                    Name = Labels.IBC_Group,
                    ChildInfo = new List<CompanyInfo>()
                };

                result.ChildInfo.Add(info);

                info = new CompanyInfo
                {
                    Id = CompanyConstants.Licensee,
                    Name = Labels.Licensee,
                    ChildInfo = new List<CompanyInfo>()
                };

                var rawData = GetReportData<dynamic>(criteria);
                if (rawData == null)
                {
                    result.ChildInfo.Add(info);
                    return result;
                }

                foreach (var row in rawData)
                {
                    info.ChildInfo.Add(new CompanyInfo
                    {
                        Id = Converter.ToInt(row.LicenseeID),
                        Name = Converter.ToString2(row.Licenseename)
                    });
                }

                result.ChildInfo.Add(info);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the bet type list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The bet type list.</returns>
        public List<EntityBase<int, string>> GetNumberGameBetTypeList(GetNumberGameBetTypeListCriteria criteria)
        {
            try
            {
                var result = new List<EntityBase<int, string>>();
                var rawData = GetReportData<dynamic>(criteria);
                if (rawData == null)
                {
                    return result;
                }

                foreach (var row in rawData)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = row.typeID,
                        Name = row.typenamee
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the company list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The company list.</returns>
        public CompanyInfo GetLiveCasinoLicenseeList(GetLiveCasinoGameCompanyListCriteria criteria)
        {
            try
            {
                var result = new CompanyInfo
                {
                    ChildInfo = new List<CompanyInfo>()
                };

                CompanyInfo info = new CompanyInfo
                {
                    Id = CompanyConstants.IBC,
                    Name = Labels.IBC_Group,
                    ChildInfo = new List<CompanyInfo>()
                };

                result.ChildInfo.Add(info);

                info = new CompanyInfo
                {
                    Id = CompanyConstants.Licensee,
                    Name = Labels.Licensee,
                    ChildInfo = new List<CompanyInfo>()
                };

                var rawData = GetReportData<dynamic>(criteria);
                if (rawData == null)
                {
                    result.ChildInfo.Add(info);
                    return result;
                }

                foreach (var row in rawData)
                {
                    info.ChildInfo.Add(new CompanyInfo
                    {
                        Id = Converter.ToInt(row.LicenseeID),
                        Name = Converter.ToString2(row.Licenseename)
                    });
                }

                result.ChildInfo.Add(info);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the company list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The company list.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public CompanyInfo GetLicenseesByProduct(GetLicenseesByProductCriteria criteria)
        {
            try
            {
                var result = new CompanyInfo
                {
                    ChildInfo = new List<CompanyInfo>()
                };

                CompanyInfo info = new CompanyInfo
                {
                    Id = CompanyConstants.IBC,
                    Name = Labels.IBC_Group,
                    ChildInfo = new List<CompanyInfo>()
                };

                result.ChildInfo.Add(info);

                info = new CompanyInfo
                {
                    Id = CompanyConstants.Licensee,
                    Name = Labels.Licensee,
                    ChildInfo = new List<CompanyInfo>()
                };

                using (var reader = _gamingRepository.GetLicenseesByProduct(criteria))
                {
                    if (reader == null)
                    {
                        result.ChildInfo.Add(info);
                        return result;
                    }

                    while (reader.Read())
                    {
                        info.ChildInfo.Add(new CompanyInfo
                        {
                            Id = Converter.ToInt(reader["LicenseeID"]),
                            Name = Converter.ToString2(reader["LicenseeName"])
                        });
                    }
                }

                result.ChildInfo.Add(info);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}
