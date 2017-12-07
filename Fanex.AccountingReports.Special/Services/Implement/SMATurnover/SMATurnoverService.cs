namespace Fanex.AccountingReports.Special.Services.Implement.SMATurnover
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Criteria.SMATurnover;
    using Fanex.AccountingReports.Special.Models.SMATurnover;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// Get data service.
    /// </summary>
    public partial class SMATurnoverService : ServiceBase, ISMATurnoverService
    {
        private string _smasTurnoverSessionKey = "Fanex.AccountingReports.Special.SMAsTurnover." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the account status.
        /// </summary>
        /// <returns>List account status information.</returns>
        public List<EntityBase<int, string>> GetAccountStatus()
        {
            try
            {
                List<EntityBase<int, string>> result = new List<EntityBase<int, string>>();
                SMAsTurnoverCollection collection = SMAsTurnoverSection.Instance.AccountStatus;
                foreach (SMAsTurnoverElement element in collection)
                {
                    if (!element.IsActive)
                    {
                        continue;
                    }

                    result.Add(new EntityBase<int, string>()
                    {
                        Id = Converter.ToInt(element.Id),
                        Name = Converter.ToString2(element.Name)
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
        /// Gets the products.
        /// </summary>
        /// <returns>List sport products.</returns>
        public List<BaseModel> GetProducts()
        {
            try
            {
                List<BaseModel> result = new List<BaseModel>();
                var sportBooksBaseModel = new BaseModel();
                SMAsTurnoverCollection collection = SMAsTurnoverSection.Instance.Products;
                foreach (SMAsTurnoverElement element in collection)
                {
                    if (!element.IsActive)
                    {
                        continue;
                    }

                    if (element.Name == "Sportsbook")
                    {
                        sportBooksBaseModel.Id = Converter.ToInt(element.Id);
                        sportBooksBaseModel.Name = Converter.ToString2(element.Name);
                        sportBooksBaseModel.AliasName = Converter.ToString2(element.AliasName);
                        continue;
                    }

                    result.Add(new BaseModel()
                    {
                        Id = Converter.ToInt(element.Id),
                        Name = Converter.ToString2(element.Name),
                        AliasName = Converter.ToString2(element.AliasName)
                    });
                }

                result = result.OrderBy(x => x.Name).ToList();
                result.Insert(0, sportBooksBaseModel);

                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the SMA turnover.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>SMA turn over data.</returns>
        public SMATurnover GetSMAsTurnover(SMATurnoverCriteria criteria)
        {
            var result = new SMATurnover
            {
                Total = new SMATurnover()
            };

            try
            {
                DataTable smasTurnoverDataTable = this.GetSMAsTurnoverData(criteria);
                var smasTurnover = this.BuildSMAsTurnoverData(smasTurnoverDataTable);
                result = this.BuildSMAsTurnoverByGroupHeadline(smasTurnover);
                SessionManager.Set<SMATurnover>(_smasTurnoverSessionKey, result);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Sort the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Data after sort.</returns>
        public SMATurnover SortReport(SMATurnoverCriteria criteria)
        {
            var report = SessionManager.Get<SMATurnover>(_smasTurnoverSessionKey);

            if (report == null)
            {
                report = this.GetSMAsTurnover(criteria);
            }

            foreach (var groupHeadline in report.ReportRows)
            {
                this.SortReportInGroupHeadline(groupHeadline, criteria);
            }

            return report;
        }

        /// <summary>
        /// Sorts the report in group headline.
        /// </summary>
        /// <param name="groupHeadline">The group headline.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Group headline data after sort.</returns>
        public SMATurnover SortReportInGroupHeadline(SMATurnover groupHeadline, SMATurnoverCriteria criteria)
        {
            bool isAsc = string.Equals(criteria.SortDirection, CoreConstants.SortDirectionConstants.Asc, StringComparison.OrdinalIgnoreCase);

            switch (criteria.SortColumnName)
            {
                case "RMTurnOver":
                    groupHeadline.ReportRows = isAsc ?
                        groupHeadline.ReportRows.OrderBy(row => row.RMTurnOver).ToList() :
                        groupHeadline.ReportRows.OrderByDescending(row => row.RMTurnOver).ToList();
                    break;
                case "MemberRMTurnOver":
                    groupHeadline.ReportRows = isAsc ?
                    groupHeadline.ReportRows.OrderBy(row => row.MemberRMTurnOver).ToList() :
                    groupHeadline.ReportRows.OrderByDescending(row => row.MemberRMTurnOver).ToList();
                    break;
                default:
                    break;
            }

            return groupHeadline;
        }

        /// <summary>
        /// Gets the SMA turnover. data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>SMA data table.</returns>
        private DataTable GetSMAsTurnoverData(SMATurnoverCriteria criteria)
        {
            DataSet ds = null;
            try
            {
                using (var objDb = new ObjectDb(criteria.GetSettingKey()))
                {
                    var _params = criteria.GetSpParams();
                    ds = objDb.ExecuteDataSet(_params);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return ds.Tables[0];
        }

        /// <summary>
        /// Build the SMA turn over data.
        /// </summary>
        /// <param name="smasTurnoverDataTable">The SMA turnover data table.</param>
        /// <returns>SMA turn over data.</returns>
        private SMATurnover BuildSMAsTurnoverData(DataTable smasTurnoverDataTable)
        {
            SMATurnover result = new SMATurnover();
            try
            {
                foreach (DataRow row in smasTurnoverDataTable.Rows)
                {
                    result.ReportRows.Add(new SMATurnover
                    {
                        GroupId = Converter.ToInt(row["GroupID"]),
                        GroupHeadline = Converter.ToString2(row["GroupHeadline"]),
                        UserName = Converter.ToString2(row["UserName"]),
                        Currency = Converter.ToString2(row["Currency"]),
                        RoleId = Converter.ToInt(row["RoleID"]),
                        TurnOver = Converter.ToDecimal(row["TurnOverAfterPT"]),
                        RMTurnOver = Converter.ToDecimal(row["TurnOverAfterPTMYR"]),
                        MemberTurnOver = Converter.ToDecimal(row["MemberTurnOver"]),
                        MemberRMTurnOver = Converter.ToDecimal(row["MemberTurnOverMYR"])
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Build the SMA turn over by group headline.
        /// </summary>
        /// <param name="smasTurnover">The SMA turnover.</param>
        /// <returns>SMA turn over by group headline.</returns>
        private SMATurnover BuildSMAsTurnoverByGroupHeadline(SMATurnover smasTurnover)
        {
            SMATurnover result = new SMATurnover();

            try
            {
                SMATurnover grandTotal = new SMATurnover
                {
                    TurnOver = smasTurnover.ReportRows.Sum(x => x.TurnOver),
                    RMTurnOver = smasTurnover.ReportRows.Sum(x => x.RMTurnOver),
                    MemberTurnOver = smasTurnover.ReportRows.Sum(x => x.MemberTurnOver),
                    MemberRMTurnOver = smasTurnover.ReportRows.Sum(x => x.MemberRMTurnOver),
                };

                result.Total = grandTotal;
                result.ReportLength = smasTurnover.ReportRows.Count;

                smasTurnover.ReportRows.GroupBy(x => x.GroupId).ToList()
                .ForEach(group =>
                {
                    var headlineGroupTotal = new SMATurnover
                    {
                        TurnOver = group.Sum(x => x.TurnOver),
                        RMTurnOver = group.Sum(x => x.RMTurnOver),
                        MemberTurnOver = group.Sum(x => x.MemberTurnOver),
                        MemberRMTurnOver = group.Sum(x => x.MemberRMTurnOver)
                    };

                    var groupByHeadline = new SMATurnover
                    {
                        Total = headlineGroupTotal,
                        GroupHeadline = group.Select(x => x.GroupHeadline).FirstOrDefault(),
                        ReportRows = group.ToList()
                    };

                    result.ReportRows.Add(groupByHeadline);
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return result;
        }
    }
}
