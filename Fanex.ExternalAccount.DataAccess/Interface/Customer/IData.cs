namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Base function for all levels.
        /// </summary>
        /// <param name="recommend">Id of current level.</param>
        /// <param name="custId">Id of selected user.</param>
        /// <param name="enabled">Enable status.</param>
        /// <param name="closed">Close status.</param>
        /// <param name="suspended">Suspend status.</param>
        /// <param name="groupName">Group name of currency.</param>
        /// <returns>DataTable object.</returns>
        DataTable GetData(int? userid, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null, string groupName = "ACCDDD");
    }
}