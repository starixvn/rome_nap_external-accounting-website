namespace Fanex.ExternalAccount.Service.Interface.Login
{
    using Fanex.ExternalAccount.DTO;
    using System;

    public interface ILoginService
    {
        Result Login(
            string username, 
            string password, 
            Request request, 
            out int custId, 
            out int onuserId, 
            out bool hasMultipleGroupsPermission);

        void Logout(int custId, string username);

        bool IsKickedOut(int custId);
        
        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDbTime();
    }
}