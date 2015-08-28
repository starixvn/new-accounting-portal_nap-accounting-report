// --------------------------------------------------------------------------
// <copyright file="MailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.19">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.MailDeliver.MailClient.API;
    using MailDeliverModel = Fanex.MailDeliver.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MailService
    {
        /// <summary>
        /// Gets the mail attachments.
        /// </summary>
        /// <param name="mailItem">The mail item.</param>
        /// <returns>The mail attachments.</returns>
        public static List<MailDeliverModel.MailAttachment> GetMailAttachments(MailItem mailItem)
        {
            List<MailDeliverModel.MailAttachment> mailAttachments = null;
            if (mailItem.Attachments != null &&
                mailItem.Attachments.Count > 0)
            {
                mailAttachments = new List<MailDeliverModel.MailAttachment>();
                foreach (var attachment in mailItem.Attachments)
                {
                    var attItem = new Fanex.MailDeliver.Model.MailAttachment
                    {
                        FileName = attachment.FileName,
                        Content = attachment.Content
                    };

                    if (attachment.ContentType.ToLower().IndexOf("excel") != -1)
                    {
                        attItem.ContentType = MailDeliverModel.FileExtentions.MS_EXCEL;
                    }
                    else if (attachment.ContentType.ToLower().IndexOf("doc") != -1)
                    {
                        attItem.ContentType = MailDeliverModel.FileExtentions.MS_DOC;
                    }
                    else
                    {
                        attItem.ContentType = MailDeliverModel.FileExtentions.IMG_GIF;
                    }

                    mailAttachments.Add(attItem);
                }
            }

            return mailAttachments;
        }

        /// <summary>
        /// Send Email utility, 4 Property need to specific are Mail To, Title, Subject, Body
        /// Other Properties will be option.
        /// </summary>
        /// <param name="mailItem">The mail item.</param>
        /// <returns>Send mail result.</returns>
        public static bool SendMail(MailItem mailItem)
        {
            try
            {
                var mailRecord = new MailDeliverModel.MailRecord
                {
                    MailTo = mailItem.To,
                    MailCc = mailItem.Cc,
                    MailBcc = mailItem.Bcc,
                    Title = mailItem.Title,
                    Subject = mailItem.Subject,
                    Body = mailItem.Body,
                    Attachments = GetMailAttachments(mailItem)
                };

                //// AltView

                if (mailItem.AltView != null &&
                    mailItem.AltView.LinkedResources.Count > 0)
                {
                    mailRecord.AlternateView = new List<MailDeliverModel.MailLinkedResource>();
                    var linkedResource = mailItem.AltView.LinkedResources;
                    foreach (var linked in linkedResource)
                    {
                        var stream = linked.ContentStream;
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        var resource = new MailDeliverModel.MailLinkedResource
                        {
                            FileName = string.Empty,
                            ImageType = MailDeliverModel.FileExtentions.IMG_GIF,
                            ContentCID = linked.ContentId,
                            StreamContent = new MemoryStream(buffer)
                        };

                        mailRecord.AlternateView.Add(resource);
                    }
                }

                //// AltView

                Task.Factory.StartNew(SendMailWithEmailService, mailRecord);
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends the mail with email service.
        /// </summary>
        /// <param name="mailRecordObject">The mail record object.</param>
        private static void SendMailWithEmailService(object mailRecordObject)
        {
            MailDeliverModel.MailRecord mailRecord = mailRecordObject as MailDeliverModel.MailRecord;
            if (mailRecord == null)
            {
                return;
            }

            try
            {
                using (var mailClient = new MailDeliver())
                {
                    mailClient.SendMail(mailRecord);
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }
        }
    }
}