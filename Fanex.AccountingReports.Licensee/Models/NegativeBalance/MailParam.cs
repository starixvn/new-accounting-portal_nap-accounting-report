// --------------------------------------------------------------------------
// <copyright file="MailParam.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="David.Tran" date="2012.11.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Models.NegativeBalance
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailParam
    {
        public MailParam()
        {
            Mails = new List<NegativeBalanceMemberMailFormat>();
        }

        public List<NegativeBalanceMemberMailFormat> Mails { get; set; }

        public string Subject { get; set; }

        public string MailContent { get; set; }

        public string ReportTitle { get; set; }
    }
}
