// -----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IUserService
    {
        IEnumerable<User> GetListUsers(string userName, int? groupId);

        User GetUserById(int userId);

        Result ResetPassword(int adminId, int userId, string password, string ip);

        Result CreateUser(int adminId, string userName, string password, string firstName, string lastName, int groupId, string ip);

        Result EditUser(int adminId, int userId, string firstName, string lastName, string ip, int groupId);

        Result UpdateStatus(int adminId, int userId, bool active, string ip);

        Result CreateExistUser(int adminId, string userName, int groupId, string ip);

        IEnumerable<Function> GetUserFunction(int userId);

        Result UpdateUserPolicy(int adminId, int userId, string oldPolicy, string policy, string ip);
    }
}