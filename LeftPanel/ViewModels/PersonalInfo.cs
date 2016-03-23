// --------------------------------------------------------------------------
// <copyright file="PersonalInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Personal information object.
    /// </summary>
    public class PersonalInfo : DownlineList
    {
        public PersonalInfo()
            : base()
        {
            IsShowProducts = new List<ObjectInfo>();
        }

        public ObjectInfo CurrentLevel { get; set; }

        public ObjectInfo Name { get; set; }

        public ObjectInfo Nickname { get; set; }

        public ObjectInfo Phone { get; set; }

        public ObjectInfo Mobile { get; set; }

        public ObjectInfo Fax { get; set; }

        public ObjectInfo CustList { get; set; }

        public ObjectInfo Status { get; set; }

        public ObjectInfo BackgroundColor { get; set; }

        public ObjectInfo IsInternal { get; set; }

        public IList<ObjectInfo> IsShowProducts { get; set; }
    }
}
