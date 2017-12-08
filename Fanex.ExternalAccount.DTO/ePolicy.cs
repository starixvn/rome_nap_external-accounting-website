namespace Fanex.ExternalAccount.DTO
{
    public enum EPolicy : int
    {
        /// <summary>
        /// Create policy.
        /// </summary>
        Create = 4,

        /// <summary>
        /// Read policy.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Update policy.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Delete policy.
        /// </summary>
        Delete = 8,

        /// <summary>
        /// None policy.
        /// </summary>
        None = 0
    }
}