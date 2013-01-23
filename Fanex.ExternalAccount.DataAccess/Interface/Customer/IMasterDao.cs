// -----------------------------------------------------------------------
// <copyright file="IMasterDao.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System.Data;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMasterDao
    {
        DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null);
    }
}