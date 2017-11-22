namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Constants;
    using Fanex.AccountingReports.Licensee.Core.Configurations;
    using Fanex.AccountingReports.Licensee.Criteria.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Repositories.Implement;
    using Fanex.AccountingReports.Licensee.Repositories.Interface;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.Common.Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NegativeBalanceService : ServiceBase, INegativeBanlance
    {
        private readonly INegativeBalanceRepository _negativeBalanceRepository;

        public NegativeBalanceService(INegativeBalanceRepository negativeBalanceRepository = null)
        {
            this._negativeBalanceRepository = negativeBalanceRepository ?? new NegativeBalanceRepository();
        }

        /// <summary>
        /// Gets the negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <returns>The negative balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NegativeBalanceModel GetNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var model = new NegativeBalanceModel();
            try
            {
                var rawData = new List<NegativeBalanceUser>();
                if (negativeBalanceCriteria.IsGetCache)
                {
                    rawData = SessionManager.Get<List<NegativeBalanceUser>>(LicenseeConstants.NegativeBalanceSessionKey + negativeBalanceCriteria.Site);
                }

                var negativeBalanceList = new List<NegativeBalance>();
                if (object.ReferenceEquals(rawData, null) || !rawData.Any())
                {
                    rawData = this._negativeBalanceRepository.GetNegativeBalanceUser(negativeBalanceCriteria).ToList();
                    SessionManager.Set<List<NegativeBalanceUser>>(LicenseeConstants.NegativeBalanceSessionKey + negativeBalanceCriteria.Site, rawData);
                }

                List<NegativeBalanceUser> _listUserTemp = new List<NegativeBalanceUser>();
                if (rawData != null && (rawData.Count() > 0))
                {
                    int from = negativeBalanceCriteria.PageSize * (negativeBalanceCriteria.PageIndex - 1);
                    int to = negativeBalanceCriteria.PageSize * negativeBalanceCriteria.PageIndex;
                    for (int i = from; i < rawData.Count && i < to; i++)
                    {
                        _listUserTemp.Add(rawData[i]);
                    }

                    NegativeBalance currentNegative = null;
                    _listUserTemp.OrderBy(negativeBalanceUser => negativeBalanceUser.Site).ForEach(data =>
                        {
                            if (currentNegative == null || currentNegative.LicenseeSite != data.Site)
                            {
                                currentNegative = new NegativeBalance
                                {
                                    LicenseeSite = data.Site,
                                    NegativeBalanceUserList = new List<NegativeBalanceUser>()
                                };
                                model.NegativeBalanceList.Add(currentNegative);
                            }

                            currentNegative.NegativeBalanceUserList.Add(data);
                        });

                    List<string> licensees = rawData.Select(s => s.Site).Distinct().ToList();
                    foreach (var licensee in licensees)
                    {
                        var defaultMail = new NegativeBalanceMemberMailFormat();
                        var mail = new NegativeBalanceMemberMailFormat();
                        bool isDefault = false;
                        bool isExist = false;
                        foreach (MailElement mailRow in LicenseeEmailSection.Instance.Mails)
                        {
                            if (mailRow.Name.ToLower() == "default" && !isDefault)
                            {
                                defaultMail.Bcc = mailRow.Bcc;
                                defaultMail.To = mailRow.To;
                                defaultMail.Cc = mailRow.Cc;
                                defaultMail.Site = mailRow.Name;
                                isDefault = true;
                            }

                            if (mailRow.Name.ToLower() == licensee.ToLower())
                            {
                                isExist = true;
                                mail.Bcc = mailRow.Bcc;
                                mail.To = mailRow.To;
                                mail.Cc = mailRow.Cc;
                                mail.Site = mailRow.Name;
                                continue;
                            }
                        }

                        if (!isExist)
                        {
                            mail = defaultMail;
                            mail.Site = licensee;
                        }

                        model.MailList.Add(mail);
                    }

                    model.TotalNegativeBalanceUser = rawData.Count();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return model;
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mailParam">The mail parameter.</param>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <returns>The failed mails.</returns>
        public NegativeBalanceMemberSendMailFail SendMail(MailParam mailParam, NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var failList = new NegativeBalanceMemberSendMailFail();
            try
            {
                //// Send Mail Example
                foreach (var mail in mailParam.Mails)
                {
                    var isSuccess = false;
                    if (!string.IsNullOrEmpty(mail.To))
                    {
                        isSuccess = MailService.SendMail(new MailItem
                        {
                            To = mail.To,
                            Title = mailParam.ReportTitle,
                            Subject = "Accounting system - Negative Balance Report for " +
                                (mail.Site == "All" ? "All Licensees" : mail.Site) +
                                " (" + DateTime.Now.ToShortDateString() + ")",
                            Body = mailParam.MailContent,
                            Attachments = new List<MailAttachment>
                            {
                                this.GetExportExcelNegativeBalanceMailAttachment(negativeBalanceCriteria, mail.Site)
                            }
                        });
                    }

                    if (!isSuccess)
                    {
                        failList.IsFail = true;
                        failList.Licensees.Add(new NegativeBalanceMemberSendMailResult()
                        {
                            IsSuccess = false,
                            Site = mail.Site,
                            ErrorMessage = Labels.SendMailFail
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return failList;
        }

        /// <summary>
        /// Gets the excel negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <param name="isSendMail">If set to <c>true</c> [is send mail].</param>
        /// <param name="site">The site value.</param>
        /// <returns>The excel negative balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ExcelFile GetExcelNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria, bool isSendMail, string site)
        {
            //// local varialbles

            int startCol = 0;
            int startRow = 2;

            var excelFile = new ExcelFile();

            var rawData = SessionManager.Get<List<NegativeBalanceUser>>(LicenseeConstants.NegativeBalanceSessionKey + negativeBalanceCriteria.Site);
            var negativeBalanceList = new List<NegativeBalance>();
            if (object.ReferenceEquals(rawData, null) || !rawData.Any())
            {
                rawData = this._negativeBalanceRepository.GetNegativeBalanceUser(negativeBalanceCriteria).ToList();
                SessionManager.Set<List<NegativeBalanceUser>>(LicenseeConstants.NegativeBalanceSessionKey + negativeBalanceCriteria.Site, rawData);
            }

            var getCache = SessionManager.Get<List<NegativeBalanceUser>>(LicenseeConstants.NegativeBalanceSessionKey + negativeBalanceCriteria.Site);
            getCache = getCache ?? this._negativeBalanceRepository.GetNegativeBalanceUser(negativeBalanceCriteria).ToList();
            var model = new NegativeBalanceModel();
            NegativeBalance currentNegative = null;
            if (isSendMail)
            {
                currentNegative = new NegativeBalance
                {
                    LicenseeSite = site,
                    NegativeBalanceUserList = new List<NegativeBalanceUser>()
                };

                getCache.OrderBy(negativeBalanceUser => negativeBalanceUser.Site)
                    .Select(s => s).ForEach(data =>
                    {
                        if (data.Site.ToLower() == site.ToLower())
                        {
                            currentNegative.NegativeBalanceUserList.Add(data);
                        }
                    });

                model.NegativeBalanceList.Add(currentNegative);
            }
            else
            {
                getCache.OrderBy(negativeBalanceUser => negativeBalanceUser.Site)
                    .Select(s => s).ForEach(data =>
                    {
                        if (currentNegative == null || currentNegative.LicenseeSite.ToLower() != data.Site.ToLower())
                        {
                            currentNegative = new NegativeBalance
                            {
                                LicenseeSite = data.Site,
                                NegativeBalanceUserList = new List<NegativeBalanceUser>()
                            };
                            model.NegativeBalanceList.Add(currentNegative);
                        }

                        currentNegative.NegativeBalanceUserList.Add(data);
                    });
            }

            var excelReport = excelFile.Worksheets.Add(string.Format("NegativeBalance"));

            //// local varialbles

            //// header

            var siteName = isSendMail ?
                    site :
                    (negativeBalanceCriteria.Site == "All" ?
                        "All Licensees" :
                        negativeBalanceCriteria.Site);
            var reportName = string.Format(
                "Accounting system - Negative Balance Report for {0} ",
                siteName);

            this.ExportExcelNegativeBalanceReportAssignHeader(
                startRow,
                startCol,
                ref excelReport,
                reportName);

            //// header

            //// content
            int nextRow = 1;
            if (model != null && model.NegativeBalanceList != null && model.NegativeBalanceList.Count > 0)
            {
                this.ExportExcelNegativeBalanceReportAssignBody(startRow, startCol, ref nextRow, ref excelReport, model);
            }
            //// content
            return excelFile;
        }

        /// <summary>
        /// Exports the excel negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        public void ExportExcelNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            try
            {
                var reportName = LicenseeResources.NegativeBalance_ReportFileName +
                    (negativeBalanceCriteria.Site == "All" ?
                    "_All_Licensees" :
                    "_" + negativeBalanceCriteria.Site);

                ExcelFile excelFile = this.GetExcelNegativeBalance(negativeBalanceCriteria, false, string.Empty);
                excelFile.ReponseStream(
                    string.Format("{0}", reportName));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Gets the export excel negative balance mail attachment.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <param name="site">The site value.</param>
        /// <returns>The export excel negative balance mail attachment.</returns>
        public MailAttachment GetExportExcelNegativeBalanceMailAttachment(NegativeBalanceCriteria negativeBalanceCriteria, string site)
        {
            ExcelFile excelFile = this.GetExcelNegativeBalance(negativeBalanceCriteria, true, site);
            var mailAttactment = new MailAttachment
            {
                Content = new MemoryStream(),
                ContentType = "excel",
                FileName = "Accounting system - Negative Balance Report.xls"
            };
            excelFile.SaveXls(mailAttactment.Content);
            return mailAttactment;
        }

        /// <summary>
        /// Exports the excel negative balance report assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelNegativeBalanceReportAssignHeader(int startRow, int startCol, ref ExcelWorksheet excelReport, string title)
        {
            var backgroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(60, 94, 165));
            backgroundHeaderFormat.Font.Color = Color.FromArgb(255, 255, 255);
            var nextCol = 0;

            excelReport.Rows[startRow].Height = 255 * 2;
            excelReport.Cells[startRow, startCol].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol].Value = Labels.UserName;
            excelReport.Columns[startCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Balance;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Currency;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Rows[startRow - 2].Height = 255 * 2;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = backgroundHeaderFormat;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value = title;
        }

        /// <summary>
        /// Exports the excel negative balance report assign body.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="model">The model.</param>
        private void ExportExcelNegativeBalanceReportAssignBody(int startRow, int startCol, ref int nextRow, ref ExcelWorksheet excelReport, NegativeBalanceModel model)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.OddRowColor, false);
            var cellStyleEven = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
            var cellStyleSite = ExcelExtensions.GetCellTextStyle(ExcelExtensions.SubTotalRowColor, false);
            cellStyle.HorizontalAlignment = cellStyleEven.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            int count = 1;

            for (int i = 0; i < model.NegativeBalanceList.Count; i++)
            {
                var nextCol = 0;
                var siteTemp = model.NegativeBalanceList[i];
                if (model.NegativeBalanceList.Count > 1)
                {
                    excelReport.Cells[startRow + nextRow, startCol].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol].Value = siteTemp.LicenseeSite;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = string.Empty;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = string.Empty;
                    nextRow++;
                }

                for (int j = 0; j < siteTemp.NegativeBalanceUserList.Count; j++)
                {
                    var style = count % 2 == 1 ? cellStyle : cellStyleEven;
                    nextCol = 0;
                    var negativeUserTmp = siteTemp.NegativeBalanceUserList[j];
                    excelReport.Cells[startRow + nextRow, startCol].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol].Value = negativeUserTmp.UserName;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = negativeUserTmp.Balance.Format(2);

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = negativeUserTmp.Currency;
                    nextRow++;
                    count++;
                }

                count++;
            }
        }
    }
}
