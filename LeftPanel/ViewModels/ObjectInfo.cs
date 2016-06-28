// --------------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    /// <summary>
    /// Object information class.
    /// </summary>
    public class ObjectInfo
    {
        /// <summary>
        /// Gets or sets key of a field.
        /// </summary>
        /// <value>System.Object object.</value>
        public object Key { get; set; }

        /// <summary>
        /// Gets or sets value for the title of a field.
        /// </summary>
        /// <value>System.Object object.</value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets title for a field.
        /// </summary>
        /// <value>System.String object.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets tooltip for title if it's content is too long.        
        /// </summary>
        /// <value>System.String object.</value>
        public string Tooltip { get; set; }

        public bool Disabled { get; set; }
    }
}
