// --------------------------------------------------------------------------
// <copyright file="MailAttachment.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Models
{
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MailAttachment
    {
        public MemoryStream Content { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
    }
}