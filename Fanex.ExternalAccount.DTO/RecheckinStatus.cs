namespace Fanex.ExternalAccount.DTO
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public enum RecheckinStatus
    {
        /// <summary>
        /// Kicked out.
        /// </summary>
        KickedOut = -1,

        /// <summary>
        /// Continue status.
        /// </summary>
        Continue = 1,

        /// <summary>
        /// Maintenance status.
        /// </summary>
        Maintenance = 2,

        /// <summary>
        /// Login status.
        /// </summary>
        Login = 3,

        /// <summary>
        /// Error or exception.
        /// </summary>
        Error = 4
    }
}