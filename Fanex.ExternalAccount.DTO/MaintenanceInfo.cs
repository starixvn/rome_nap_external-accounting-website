namespace Fanex.ExternalAccount.DTO
{
    using System;
    using System.Globalization;

    public class MaintenanceInfo
    {
        public MaintenanceInfo()
        {
            this.IsDownTime = false;
            this.TimeFrom = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            this.TimeTo = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checking server is under downtime
        /// </summary>
        public bool IsDownTime { get; set; }

        /// <summary>
        /// Get from time
        /// </summary>
        public string TimeFrom { get; set; }

        /// <summary>
        /// Get to time
        /// </summary>
        public string TimeTo { get; set; }
    }
}