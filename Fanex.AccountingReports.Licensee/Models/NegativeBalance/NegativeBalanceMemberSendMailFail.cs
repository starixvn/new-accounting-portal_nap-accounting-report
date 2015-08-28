// --------------------------------------------------------------------------
// <copyright file="NegativeBalanceMemberSendMailFail.cs" company="Nexcel Solutions Vietnam">
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
    [Serializable]
    public class NegativeBalanceMemberSendMailFail
    {
        public NegativeBalanceMemberSendMailFail()
        {
            Licensees = new List<NegativeBalanceMemberSendMailResult>();
        }

        public bool IsFail { get; set; }

        public List<NegativeBalanceMemberSendMailResult> Licensees { get; set; }
    }
}
