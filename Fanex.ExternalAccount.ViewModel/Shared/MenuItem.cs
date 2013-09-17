// --------------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// Menu Item object.
    /// </summary>
    public class MenuItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<MenuItem> Children { get; set; }
    }
}