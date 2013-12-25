// -----------------------------------------------------------------------
// <copyright file="StatusKickedOut.cs" company="Nexcel Solution Co.Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DTO
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public enum StatusKickedOut
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