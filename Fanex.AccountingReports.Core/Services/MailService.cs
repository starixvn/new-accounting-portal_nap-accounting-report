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

                    if (attachment.ContentType.ToLower().IndexOf("excel", StringComparison.Ordinal) != -1)
                    {
                        attItem.ContentType = MailDeliverModel.FileExtentions.MS_EXCEL;
                    }
                    else if (attachment.ContentType.ToLower().IndexOf("doc", StringComparison.Ordinal) != -1)
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
                    var resource = new MailDeliverModel.MailLinkedResource
                    {
                        FileName = string.Empty,
                        ImageType = MailDeliverModel.FileExtentions.IMG_GIF,
                        ContentCID = linked.ContentId,
                        StreamContent = CreateMemoryStream(stream)
                    };

                    mailRecord.AlternateView.Add(resource);
                }
            }

            //// AltView

            Task.Factory.StartNew(SendMailWithEmailService, mailRecord);

            return true;
        }

        private static MemoryStream CreateMemoryStream(Stream stream)
        {
            var buffer = new byte[stream.Length];
            var ms = new MemoryStream();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms;
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

            using (var mailClient = new MailDeliver())
            {
                mailClient.SendMail(mailRecord);
            }
        }
    }
}