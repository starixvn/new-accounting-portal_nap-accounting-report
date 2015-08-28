// --------------------------------------------------------------------------
// <copyright file="AGCasinoWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Models.AGCasinoWinloss;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.AccountingReports.Licensee.Utilities.AGCasinoWinloss;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AGCasinoWinLossService : ServiceBase, IAGCasinoWinLossService
    {
        private IDataReader _reader;

        private List<AGCasinoWinloss> _totalBlock;

        private List<AGCasinoWinloss> _grandTotalBlock;

        /// <summary>
        /// Licensees this instance.
        /// </summary>
        /// <returns>List Select list items.</returns>
        public List<Licensee> Licensee()
        {
            List<Licensee> result = new List<Licensee>();
            using (var db = new ObjectDb(new GetLicenseeCriteria().GetSettingKey()))
            {
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Licensee()
                    {
                        LicenseeID = reader.GetInt32(0),
                        LicenseeName = reader.GetString(1),
                        BrandName = reader.GetString(2),
                        LaunchDate = reader.GetDateTime(3).ToString(FormatConstants.MMMDDYYYYString)
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Currencies the by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List select list item.</returns>
        public List<SelectListItem> CurrencyByLicensee(GetCurrencyByLicensee criteria)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var reader = db.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new SelectListItem()
                    {
                        Value = reader[0].ToString(),
                        Text = reader[1].ToString()
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the ag casino win loss main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Data Collection.</returns>
        public AGCasinoWinlossData GetAGCasinoWinlossMainReport(GetMainReportCriteria criteria)
        {
            AGCasinoWinlossData mainReport = new AGCasinoWinlossData();
            if (criteria.IsRefresh == 1)
            {
                mainReport = this.GetReportFromSP(criteria);
            }
            else
            {
                mainReport = SessionManager.Get<AGCasinoWinlossData>(LicenseeConstants.AGCasinoWinLossMainReportSessionKey);
                if (mainReport == null)
                {
                    mainReport = this.GetReportFromSP(criteria);
                }
            }

            mainReport.TotalItem = mainReport.Data.Count;
            SessionManager.Set<AGCasinoWinlossData>(LicenseeConstants.AGCasinoWinLossMainReportSessionKey, mainReport);
            return this.GetReportWithPaging(criteria, mainReport);
        }

        /// <summary>
        /// Exports the ag casino win loss main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportAGCasinoWinlossMainReport(GetMainReportCriteria criteria)
        {
            AGCasinoWinlossData data = new AGCasinoWinlossData();
            if (criteria.IsRefresh == 1)
            {
                data = this.GetReportFromSP(criteria);
            }
            else
            {
                data = SessionManager.Get<AGCasinoWinlossData>(LicenseeConstants.AGCasinoWinLossMainReportSessionKey);
                if (data == null)
                {
                    data = this.GetReportFromSP(criteria);
                }
            }

            var excelUtil = new ExcelUtility(data, criteria);
            NpoiExcelExtensions.ResponseStreamExcel(excelUtil.ExportExcel(), this.GetFileName(criteria));
        }

        /// <summary>
        /// Exports the ag casino win loss SMAM report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void ExportAGCasinoWinlossSMAMReport(GetMainReportCriteria criteria)
        {
            AGCasinoWinlossData data = new AGCasinoWinlossData();
            if (criteria.IsRefresh == 1)
            {
                data = this.GetReportFromSP(criteria);
            }
            else
            {
                data = SessionManager.Get<AGCasinoWinlossData>(this.GetSessionKey(criteria));
                if (data == null)
                {
                    data = this.GetReportFromSP(criteria);
                }
            }

            var excelUtil = new ExcelUtility(data, criteria);
            NpoiExcelExtensions.ResponseStreamExcel(excelUtil.ExportExcel(), this.GetFileName(criteria));
        }

        /// <summary>
        /// Gets the ag casino win loss SMAM report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Data Collection.</returns>
        public AGCasinoWinlossData GetAGCasinoWinlossSMAMReport(GetMainReportCriteria criteria)
        {
            AGCasinoWinlossData smamReport = new AGCasinoWinlossData();
            string sessionKey = this.GetSessionKey(criteria);
            if (criteria.IsRefresh == 1)
            {
                smamReport = this.GetReportFromSP(criteria);
            }
            else
            {
                smamReport = SessionManager.Get<AGCasinoWinlossData>(sessionKey);
                if (smamReport == null)
                {
                    smamReport = this.GetReportFromSP(criteria);
                }
            }

            smamReport.TotalItem = smamReport.Data.Count;
            SessionManager.Set<AGCasinoWinlossData>(this.GetSessionKey(criteria), smamReport);
            return this.GetReportWithPaging(criteria, smamReport);
        }

        /// <summary>
        /// Gets the report from SP.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Data Collection.</returns>
        private AGCasinoWinlossData GetReportFromSP(GetMainReportCriteria criteria)
        {
            AGCasinoWinlossData mainReport = new AGCasinoWinlossData();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                this._reader = db.ExecuteReader();
                while (this._reader.Read())
                {
                    AGCasinoWinloss mainRow;
                    if (criteria.GetType().Equals(typeof(GetMainReportCriteria)))
                    {
                        mainRow = this.MapAGCasinoWinlossMain(criteria);
                    }
                    else
                    {
                        mainRow = this.MAPAGCasinoWinlossSMAM(criteria);
                    }

                    mainReport.Data.Add(mainRow);
                    this.AddValueToTotalBlock(mainRow);
                    this.AddValueToGrandTotalBlock(criteria);
                }
            }

            if (this._totalBlock != null && this._totalBlock.Count > 0)
            {
                mainReport.Total = this._totalBlock;
            }

            if (this._grandTotalBlock != null && this._grandTotalBlock.Count > 0)
            {
                mainReport.GrandTotal = this._grandTotalBlock;
            }

            return mainReport;
        }

        /// <summary>
        /// Gets the session key.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Session string.</returns>
        private string GetSessionKey(GetMainReportCriteria criteria)
        {
            var criteriaType = criteria.GetType();
            if (criteriaType.Equals(typeof(GetMainReportCriteria)))
            {
                return LicenseeConstants.AGCasinoWinLossMainReportSessionKey;
            }
            else if (criteriaType.Equals(typeof(GetSuperReportCriteria)))
            {
                return LicenseeConstants.AGCasinoWinLossSuperReportSessionKey;
            }
            else if (criteriaType.Equals(typeof(GetMasterReportCriteria)))
            {
                return LicenseeConstants.AGCasinoWinLossMasterReportSessionKey;
            }
            else if (criteriaType.Equals(typeof(GetAgentReportCriteria)))
            {
                return LicenseeConstants.AGCasinoWinLossAgentReportSessionKey;
            }
            else if (criteriaType.Equals(typeof(GetMemberReportCriteria)))
            {
                return LicenseeConstants.AGCasinoWinLossMemberReportSessionKey;
            }
            else
            {
                return LicenseeConstants.AGCasinoWinLossMainReportSessionKey;
            }
        }

        private AGCasinoWinlossData GetReportWithPaging(GetMainReportCriteria criteria, AGCasinoWinlossData mainReport)
        {
            AGCasinoWinlossData mainReportWithPaging = new AGCasinoWinlossData();
            mainReportWithPaging.Total = mainReport.Total;
            mainReportWithPaging.GrandTotal = mainReport.GrandTotal;
            mainReportWithPaging.Data = mainReport.Data.Skip(criteria.PageSize * criteria.PageIndex).Take(criteria.PageSize).ToList();
            mainReportWithPaging.TotalItem = mainReport.TotalItem;
            return mainReportWithPaging;
        }

        /// <summary>
        /// Maps the ag casino win loss main.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Data Object.</returns>
        private AGCasinoWinloss MapAGCasinoWinlossMain(GetMainReportCriteria criteria)
        {
            int shiftColumns = criteria.GetType().Equals(typeof(GetMainReportCriteria)) ? 0 : 1;
            AGCasinoWinloss result = new AGCasinoWinloss();
            int currentColumn = 0;
            if (shiftColumns == 0)
            {
                result.Date = this._reader.GetDateTime(currentColumn).ToString(criteria.ReportType == 1 ? "MM/dd/yyyy" : "MM/yyyy");
            }

            currentColumn++;
            result.CurrencyID = System.Convert.ToInt32(this._reader.GetInt32(currentColumn++ + shiftColumns));
            result.CurrencyName = this._reader.GetString(currentColumn++ + shiftColumns);
            result.Turnover = System.Convert.ToDouble(this._reader[currentColumn++ + shiftColumns + criteria.ConvertFiguresTo]);
            result.TotalTrans = System.Convert.ToInt32(this._reader[4 + shiftColumns]);
            if (criteria.ConvertFiguresTo == 0)
            {
                currentColumn++;
            }

            result.CustomerWinLoss = System.Convert.ToDouble(this._reader[currentColumn++ + shiftColumns + criteria.ConvertFiguresTo]);
            result.LicenseeWinLoss = System.Convert.ToDouble(this._reader[currentColumn++ + shiftColumns + criteria.ConvertFiguresTo]);
            result.AmountDueToAsiaGamings = System.Convert.ToDouble(this._reader[currentColumn++ + shiftColumns + criteria.ConvertFiguresTo]);
            return result;
        }

        private AGCasinoWinloss MAPAGCasinoWinlossSMAM(GetMainReportCriteria criteria)
        {
            var result = this.MapAGCasinoWinlossMain(criteria);
            result.CustID = this._reader.GetInt32(0);
            result.UserName = this._reader.GetString(1);
            return result;
        }

        /// <summary>
        /// Adds the value to total block.
        /// </summary>
        /// <param name="row">The data row.</param>
        private void AddValueToTotalBlock(AGCasinoWinloss row)
        {
            if (this._totalBlock == null)
            {
                this._totalBlock = new List<AGCasinoWinloss>();
                this._totalBlock.Add(new AGCasinoWinloss(row));
                this._totalBlock[0].Date = Labels.Total;
                this._totalBlock[0].UserName = Labels.Total;
                return;
            }

            int totalBlockCount = this._totalBlock.Count;
            for (int i = 0; i < totalBlockCount; i++)
            {
                if (this._totalBlock[i].CurrencyID == row.CurrencyID)
                {
                    this._totalBlock[i].Turnover += row.Turnover;
                    this._totalBlock[i].TotalTrans += row.TotalTrans;
                    this._totalBlock[i].CustomerWinLoss += row.CustomerWinLoss;
                    this._totalBlock[i].LicenseeWinLoss += row.LicenseeWinLoss;
                    this._totalBlock[i].AmountDueToAsiaGamings += row.AmountDueToAsiaGamings;
                    return;
                }
            }

            this._totalBlock.Add(new AGCasinoWinloss(row));
            this._totalBlock[this._totalBlock.Count - 1].Date = Labels.Total;
            this._totalBlock[this._totalBlock.Count - 1].UserName = Labels.Total;
        }

        /// <summary>
        /// Adds the value to grand total block.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed.")]
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        private void AddValueToGrandTotalBlock(GetMainReportCriteria criteria)
        {
            int shiftColumns = criteria.GetType().Equals(typeof(GetMainReportCriteria)) ? 0 : 1;
            AGCasinoWinloss MYRGrandTotalRow;
            AGCasinoWinloss EURrandTotalRow;
            AGCasinoWinloss USDGrandTotalRow;
            AGCasinoWinloss GBPGrandTotalRow;
            if (this._grandTotalBlock == null)
            {
                this._grandTotalBlock = new List<AGCasinoWinloss>();
                MYRGrandTotalRow = new AGCasinoWinloss();
                USDGrandTotalRow = new AGCasinoWinloss();
                EURrandTotalRow = new AGCasinoWinloss();
                GBPGrandTotalRow = new AGCasinoWinloss();
                MYRGrandTotalRow.Date = USDGrandTotalRow.Date = EURrandTotalRow.Date = GBPGrandTotalRow.Date = Labels.Total;
                MYRGrandTotalRow.UserName = USDGrandTotalRow.UserName = EURrandTotalRow.UserName = GBPGrandTotalRow.UserName = Labels.Total;
                MYRGrandTotalRow.CurrencyName = Labels.MYR;
                USDGrandTotalRow.CurrencyName = Labels.USD;
                EURrandTotalRow.CurrencyName = Labels.EUR;
                GBPGrandTotalRow.CurrencyName = Labels.GBP;
                this._grandTotalBlock.Add(MYRGrandTotalRow);
                this._grandTotalBlock.Add(EURrandTotalRow);
                this._grandTotalBlock.Add(USDGrandTotalRow);
                this._grandTotalBlock.Add(GBPGrandTotalRow);
            }
            else
            {
                MYRGrandTotalRow = this._grandTotalBlock[0];
                EURrandTotalRow = this._grandTotalBlock[1];
                USDGrandTotalRow = this._grandTotalBlock[2];
                GBPGrandTotalRow = this._grandTotalBlock[3];
            }

            MYRGrandTotalRow.Turnover += System.Convert.ToDouble(this._reader[8 + shiftColumns]);
            MYRGrandTotalRow.TotalTrans += System.Convert.ToInt32(this._reader[4 + shiftColumns]);
            MYRGrandTotalRow.CustomerWinLoss += System.Convert.ToDouble(this._reader[9 + shiftColumns]);
            MYRGrandTotalRow.LicenseeWinLoss += System.Convert.ToDouble(this._reader[10 + shiftColumns]);
            MYRGrandTotalRow.AmountDueToAsiaGamings += System.Convert.ToDouble(this._reader[11 + shiftColumns]);

            USDGrandTotalRow.Turnover += System.Convert.ToDouble(this._reader[12 + shiftColumns]);
            USDGrandTotalRow.TotalTrans += System.Convert.ToInt32(this._reader[4 + shiftColumns]);
            USDGrandTotalRow.CustomerWinLoss += System.Convert.ToDouble(this._reader[13 + shiftColumns]);
            USDGrandTotalRow.LicenseeWinLoss += System.Convert.ToDouble(this._reader[14 + shiftColumns]);
            USDGrandTotalRow.AmountDueToAsiaGamings += System.Convert.ToDouble(this._reader[15 + shiftColumns]);

            EURrandTotalRow.Turnover += System.Convert.ToDouble(this._reader[16 + shiftColumns]);
            EURrandTotalRow.TotalTrans += System.Convert.ToInt32(this._reader[4 + shiftColumns]);
            EURrandTotalRow.CustomerWinLoss += System.Convert.ToDouble(this._reader[17 + shiftColumns]);
            EURrandTotalRow.LicenseeWinLoss += System.Convert.ToDouble(this._reader[18 + shiftColumns]);
            EURrandTotalRow.AmountDueToAsiaGamings += System.Convert.ToDouble(this._reader[19 + shiftColumns]);

            GBPGrandTotalRow.Turnover += System.Convert.ToDouble(this._reader[20 + shiftColumns]);
            GBPGrandTotalRow.TotalTrans += System.Convert.ToInt32(this._reader[4 + shiftColumns]);
            GBPGrandTotalRow.CustomerWinLoss += System.Convert.ToDouble(this._reader[21 + shiftColumns]);
            GBPGrandTotalRow.LicenseeWinLoss += System.Convert.ToDouble(this._reader[22 + shiftColumns]);
            GBPGrandTotalRow.AmountDueToAsiaGamings += System.Convert.ToDouble(this._reader[23 + shiftColumns]);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>File name.</returns>
        private string GetFileName(GetMainReportCriteria criteria)
        {
            string fileName = string.Empty;
            var criteriaType = criteria.GetType();
            if (criteriaType.Equals(typeof(GetMainReportCriteria)) || criteriaType.Equals(typeof(GetSuperReportCriteria)))
            {
                fileName = string.Format(
                    Labels.AGCasinoWinlossFileName,
                    criteria.LicenseeName,
                    criteria.FromDate.ToString("MM_dd_yyyy"),
                    criteria.ToDate.ToString("MM_dd_yyyy"),
                    criteria.CurrencyName.Replace(" ", "_"));
            }
            else
            {
                var masterCriteria = (GetMasterReportCriteria)criteria;
                fileName = string.Format(
                       Labels.AGCasinoWinlossFileName,
                       masterCriteria.CustomerName,
                       masterCriteria.FromDate.ToString("MM_dd_yyyy"),
                       masterCriteria.ToDate.ToString("MM_dd_yyyy"),
                       masterCriteria.CurrencyName.Replace(" ", "_"));
            }

            return fileName;
        }
    }
}