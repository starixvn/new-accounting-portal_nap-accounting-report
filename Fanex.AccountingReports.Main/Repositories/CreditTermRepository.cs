namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Models.CreditTerm;
    using Fanex.Common;
    using Fanex.Data;

    public class CreditTermRepository
    {
        public List<CreditTerm> GetCreditTerm(GetCreditTermCriteria criteria)
        {
            var report = new List<CreditTerm>();
            var settingKey = criteria.GetSettingKey();

            using (var db = ObjectDbFactory.CreateInstance(settingKey))
            {
                DataSet multipleResult = db.ExecuteDataSet(criteria);

                if (multipleResult != null && multipleResult.Tables.Count >= 2)
                {
                    report = this.LoopAndConvertDataIntoReport(multipleResult, report);
                }
            }

            return report;
        }

        public EditedRemarkResult EditRemark(GetRemarkEditedCriteria criteria)
        {
            var editedRemark = new EditedRemarkResult();
            var settingKey = criteria.GetSettingKey();

            if (criteria.AccountRemark == null)
            {
                criteria.AccountRemark = string.Empty;
            }

            using (var objectdb = ObjectDbFactory.CreateInstance(settingKey, 500))
            {
                objectdb.ExecuteNonQuery(criteria);

                editedRemark.ErrorCode = Converter.ToInt(objectdb.GetParameterValue("errcode"));
                editedRemark.ErrorMessage = Converter.ToString2(objectdb.GetParameterValue("errmsg"));
                editedRemark.AccountEditedRemarkDate =
                    Converter.ToString2(objectdb.GetParameterValue("accountremarkdate"));

                return editedRemark;
            }
        }

        
        private List<CreditTerm> LoopAndConvertDataIntoReport(
            DataSet multipleResult,
            List<CreditTerm> report)
        {
            var groupRows = multipleResult.Tables[0].Rows;
            var memberRows = multipleResult.Tables[1].AsEnumerable();

            GetCurrencyByLicenseeCriteria criteria = new GetCurrencyByLicenseeCriteria();
            var currencies = ReportService.GetCurrencyByLicensee(criteria);

            if (groupRows != null && groupRows.Count > 0)
            {
                foreach (DataRow parentRow in groupRows)
                {
                    var groupID = parentRow["groupID"];
                    var currencyID = Converter.ToInt(parentRow["currency"]);
                    var currencyName = currencies.Where(s => s.Id == currencyID)
                                                 .Select(s => s.Name).FirstOrDefault();

                    if (currencyName == null)
                    {
                        currencyName = currencyID == 20 ? "UUS" : string.Empty;
                    }

                    var parentMember = new CreditTerm
                    {
                        GroupID = Converter.ToInt(groupID),
                        RoleName = Converter.ToString2(parentRow["Role"]),
                        Username = Converter.ToString2(parentRow["Username"]),
                        CurrencyName = currencyName,
                        PreAgreeAmount = Convert.ToDouble(parentRow["PreAgreeAmount"]),
                        Balance = Convert.ToDouble(parentRow["Balance"]),
                        BalanceBase = Convert.ToDouble(parentRow["Balance_Base"]),
                        Reward = Convert.ToDouble(parentRow["Reward"]),
                        RewardRM = Convert.ToDouble(parentRow["RewardRM"]),
                        AccountRemark = Converter.ToString2(parentRow["AccountRemark"]),
                        PhoneNumber = Converter.ToString2(parentRow["phone"]),
                        CheckerName = Converter.ToString2(parentRow["checkername"]),
                        AccountRemarkDate = Converter.ToString2(parentRow["AccountRemarkDate"]),
                        OverLimit = Convert.ToBoolean(parentRow["OverLimit"])
                    };

                    if (memberRows != null)
                    {
                        var membersOfGroup = memberRows.Where(row => row["groupID"].Equals(groupID))
                                                       .Distinct().ToList();

                        if (membersOfGroup != null && membersOfGroup.Count > 0)
                        {
                            foreach (DataRow memberRow in membersOfGroup)
                            {
                                var memberCurrencyId = Converter.ToInt(memberRow["currency"]);
                                var memberCurrencyName = currencies.Where(s => s.Id == memberCurrencyId)
                                                                   .Select(s => s.Name).FirstOrDefault();

                                if (memberCurrencyName == null)
                                {
                                    memberCurrencyName = memberCurrencyId == 20 ? "UUS" : string.Empty;
                                }

                                var member = new CreditTerm
                                {
                                    RoleName = Converter.ToString2(memberRow["Role"]),
                                    Username = Converter.ToString2(memberRow["Username"]),
                                    CustomerID = Converter.ToInt(memberRow["custid"]),
                                    CurrencyName = memberCurrencyName,
                                    Balance = Convert.ToDouble(memberRow["Balance"]),
                                    BalanceBase = Convert.ToDouble(memberRow["Balance_Base"]),
                                    Reward = Convert.ToDouble(memberRow["Reward"]),
                                    RewardRM = Convert.ToDouble(memberRow["RewardRM"]),
                                    BalanceGroup = Convert.ToDouble(memberRow["Balance_Group"]),
                                    AccountRemark = Converter.ToString2(memberRow["AccountRemark"]),
                                    AccountRemarkDate = Converter.ToString2(memberRow["AccountRemarkDate"])
                                };

                                parentMember.Rows.Add(member);
                            }

                            report.Add(parentMember);
                        }
                    }
                }
            }

            return report;
        }
    }
}
