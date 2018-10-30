namespace Fanex.AccountingReports.Misc.Services.Implement.SMATurnover
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.SMATurnover;
    using Fanex.AccountingReports.Misc.Models.SMATurnover;
    using Fanex.Common;
    using Fanex.Data;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class SMATurnoverService : ServiceBase
    {
        private readonly string _smasTurnoverSessionKey = "Fanex.AccountingReports.Misc.SMATurnover." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the account status.
        /// </summary>
        /// <returns>Account status.</returns>
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
        /// <returns>The product items.</returns>
        public List<BaseModel> GetProducts()
        {
            try
            {
                List<BaseModel> result = new List<BaseModel>();
                SMAsTurnoverCollection collection = SMAsTurnoverSection.Instance.Products;
                foreach (SMAsTurnoverElement element in collection)
                {
                    if (!element.IsActive)
                    {
                        continue;
                    }

                    result.Add(new BaseModel()
                    {
                        Id = Converter.ToInt(element.Id),
                        Name = Converter.ToString2(element.Name),
                        AliasName = Converter.ToString2(element.AliasName)
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
        /// Gets the SMA turn over.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>SMA turn over data.</returns>
        public SMATurnover GetSMAsTurnover(GetSMATurnoverReportCriteria criteria)
        {
            IDataReader smasTurnoverDataReader = null;
            var result = new SMATurnover
            {
                Total = new SMATurnover()
            };

            try
            {
                smasTurnoverDataReader = this.GetSMAsTurnoverData(criteria);
                var smasTurnover = this.BuildSMAsTurnoverData(smasTurnoverDataReader);
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
        /// Sorts the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>SMA turn over data after sorted.</returns>
        public SMATurnover SortReport(GetSMATurnoverReportCriteria criteria)
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
        public SMATurnover SortReportInGroupHeadline(SMATurnover groupHeadline, GetSMATurnoverReportCriteria criteria)
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
        /// Gets the SMA turnover data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>SMA turn over data reader.</returns>
        private IDataReader GetSMAsTurnoverData(GetSMATurnoverReportCriteria criteria)
        {
            IDataReader reader = null;
            try
            {
                var objDb = new ObjectDb(criteria.GetSettingKey());
                var _params = criteria.GetSpParams();
                reader = objDb.ExecuteReader(_params);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            return reader;
        }

        /// <summary>
        /// Build the SMA turn over data.
        /// </summary>
        /// <param name="reader">The SMA turnover data reader.</param>
        /// <returns>SMA turn over data.</returns>
        private SMATurnover BuildSMAsTurnoverData(IDataReader reader)
        {
            SMATurnover result = new SMATurnover();
            try
            {
                while (reader.Read())
                {
                    result.ReportRows.Add(new SMATurnover
                    {
                        GroupId = Converter.ToInt(reader["GroupID"]),
                        GroupHeadline = Converter.ToString2(reader["GroupHeadline"]),
                        UserName = Converter.ToString2(reader["UserName"]),
                        Currency = Converter.ToString2(reader["Currency"]),
                        RoleId = Converter.ToInt(reader["RoleID"]),
                        TurnOver = Converter.ToDecimal(reader["TurnOverAfterPT"]),
                        RMTurnOver = Converter.ToDecimal(reader["TurnOverAfterPTMYR"]),
                        MemberTurnOver = Converter.ToDecimal(reader["MemberTurnOver"]),
                        MemberRMTurnOver = Converter.ToDecimal(reader["MemberTurnOverMYR"])
                    });
                }

                reader.Close();
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