namespace Fanex.ExternalAccount.DataAccess.Impl.Login
{
    using System;
    using System.Collections;
    using System.Data;
    using Common;
    using Data;
    using DTO;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Login;
    using Interface.Login;

    public class LoginDao : ILoginDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public LoginDao() : this(null)
        {
        }

        public LoginDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Result Login(
            string username,
            string password,
            string browser,
            string ip,
            string domain,
            string countryCode,
            string platForm,
            out int custId)
        {
            var command = new LoginCommand(username, password, browser, ip, domain, countryCode, platForm);
            _dynamicRepository.Execute(command);

            custId = command.CustId;
            return new Result(command.ErrCode, command.ErrMsg, command.GroupID);
        }

        public void Logout(int custId, string username)
            => _dynamicRepository.Execute(new LogoutCommand(custId, username));

        public int Checkin(int custId, string netSessionId, string ip)
        {
            var command = new CheckinCommand(custId, netSessionId, ip);
            _dynamicRepository.Execute(command);
            return command.OnUserId;
        }

        public bool IsKickedOut(int custId)
        {
            var command = new IsKickedOutCommand(custId);
            _dynamicRepository.Execute(command);
            return command.IsKickedOut;
        }

        public bool CheckExternalGroup(int custId, int? funcId, string funcKey)
        {
            var command = new CheckExternalGroupCommand(custId, funcId, funcKey);
            _dynamicRepository.Execute(command);
            var result = new Result(command.ErrCode, command.ErrorMsg);
            return result.IsSuccess;
        }

        public DateTime GetDbTime()
        {
            var command = new GetDbTimeCommand();
            _dynamicRepository.Execute(command);
            return command.LoginTime;
        }
    }
}