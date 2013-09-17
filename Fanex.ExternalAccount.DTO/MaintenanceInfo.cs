// --------------------------------------------------------------------------
// <copyright file="MaintenanceInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.DTO
{
    using System;
    using System.Globalization;

    /// <summary>
    /// MaintenanceInfo class.
    /// </summary>
    public class MaintenanceInfo
    {
        public MaintenanceInfo()
        {
            this.IsDownTime = false;
            this.TimeFrom = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            this.TimeTo = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets or sets a value indicating whether server is under downtime or not.
        /// </summary>
        /// <value>Boolean object.</value>
        public bool IsDownTime { get; set; }

        /// <summary>
        /// Gets or sets from time.
        /// </summary>
        /// <value>String object.</value>
        public string TimeFrom { get; set; }

        /// <summary>
        /// Gets or sets to time.
        /// </summary>
        /// <value>String object.</value>
        public string TimeTo { get; set; }
    }
}