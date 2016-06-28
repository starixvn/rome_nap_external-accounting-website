// --------------------------------------------------------------------------
// <copyright file="Login.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// Login object.
    /// </summary>
    public class Login : OnlineInfo
    {
        public ObjectInfo Site { get; set; }

        public ObjectInfo Password { get; set; }

        public ObjectInfo LoginOption { get; set; }

        public ObjectInfo SecurityCode { get; set; }

        public ObjectInfo CreatedDate { get; set; }

        public ObjectInfo LastTrans { get; set; }
    }
}
