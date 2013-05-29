// --------------------------------------------------------------------------
// <copyright file="MailItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
// </history>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Mail;

namespace Fanex.Puffin.Core.Models
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class MailItem
    {
        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public IList<MailAttachment> Attachments { get; set; }

        public AlternateView AltView { get; set; }
    }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class MailAttachment
    {
        public MemoryStream Content { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
    }
}