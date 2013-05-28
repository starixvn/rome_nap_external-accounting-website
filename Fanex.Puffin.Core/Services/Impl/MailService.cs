// --------------------------------------------------------------------------
// <copyright file="MailService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.19">Created</change>
// </history>

using System;
using System.Collections.Generic;
using System.IO;
using Fanex.MailDeliver.Model;
using System.Threading.Tasks;
using Fanex.Puffin.Core.Models;

namespace Fanex.Puffin.Core.Services.Impl
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class MailService
    {
        /// <summary>
        /// Send Email Util, 4 Property need to specific are Mail To, Title, Subject, Body
        /// Other Properties will be option
        /// </summary>
        /// <param name="mailItem"></param>
        public static bool SendMail(MailItem mailItem)
        {
            try
            {
                #region Send Mail Example
                //#region test attachment
                //var stream = System.IO.File.Open(HttpContext.Current.Request.MapPath(".") + "\\" + "App_Data" + "\\Licensee-NumberGameWinLoss_From_7_29_2012_To_10_31_2012.xls", FileMode.Open);
                //var buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);
                //stream.Flush();
                //stream.Close();
                ////#endregion test attachment
                //MailService.SendMail(new MailItem
                //{
                //    To = "ricky.nguyen@nexcel.vn",
                //    Title = "Test mail",
                //    Subject = "Test mail",
                //    Body = "Test mail",
                //    Attachments = new List<MailAttachment>
                //{
                //    new MailAttachment
                //    {
                //        Content = new MemoryStream(buffer),
                //        ContentType = "excel",
                //        FileName = "Licensee"
                //    }
                //}
                //});
                #endregion

                var mailRecord = new MailRecord
                {
                    MailTo = mailItem.To,
                    MailCc = mailItem.Cc,
                    MailBcc = mailItem.Bcc,
                    Title = mailItem.Title,
                    Subject = mailItem.Subject,
                    Body = mailItem.Body
                };

                #region Add Attachments File

                if (mailItem.Attachments != null
                    && mailItem.Attachments.Count > 0)
                {
                    mailRecord.Attachments = new List<Fanex.MailDeliver.Model.MailAttachment>();

                    foreach (var attachment in mailItem.Attachments)
                    {
                        var attItem = new Fanex.MailDeliver.Model.MailAttachment
                        {
                            FileName = attachment.FileName,
                            Content = attachment.Content
                        };
                        if (attachment.ContentType.ToLower().IndexOf("excel") != -1)
                        {
                            attItem.ContentType = FileExtentions.MS_EXCEL;
                        }
                        else if (attachment.ContentType.ToLower().IndexOf("doc") != -1)
                        {
                            attItem.ContentType = FileExtentions.MS_DOC;
                        }
                        else
                        {
                            attItem.ContentType = FileExtentions.IMG_GIF;
                        }

                        mailRecord.Attachments.Add(attItem);
                    }
                }

                #endregion Add Attachments File

                #region AltView

                if (mailItem.AltView != null
                    && mailItem.AltView.LinkedResources.Count > 0)
                {
                    mailRecord.AlternateView = new List<MailLinkedResource>();
                    var linkedResource = mailItem.AltView.LinkedResources;
                    foreach (var linked in linkedResource)
                    {
                        var stream = linked.ContentStream;
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        var resource = new MailLinkedResource
                        {
                            FileName = string.Empty,
                            ImageType = FileExtentions.IMG_GIF,
                            ContentCID = linked.ContentId,
                            StreamContent = new MemoryStream(buffer)
                        };
                        mailRecord.AlternateView.Add(resource);
                    }
                }

                #endregion AltView

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
        /// Send mail Through Nexcel Mail Service
        /// </summary>
        /// <param name="mailRecordObject"></param>
        private static void SendMailWithEmailService(object mailRecordObject)
        {
            MailRecord mailRecord = mailRecordObject as MailRecord;
            if (mailRecord != null)
            {
                try
                {
                    using (var mailClient = new MailDeliver.MailClient.API.MailDeliver())
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
}