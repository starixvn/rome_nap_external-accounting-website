namespace Fanex.ExternalAccount.DTO.Customer.Status
{
    using Core.Shared;

    public class UnsuspendDownlineDetail
    {
        public UnsuspendDownlineDetail(int roleId, string userName)
        {
            RoleId = roleId;
            UserName = userName;
        }

        public int RoleId { get; }

        public string LevelName
        {
            get
            {
                return Role.DisplayRoleName(RoleId);
            }
        }

        public string UserName { get; }
    }
}
