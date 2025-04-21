using Fanex.ProductStatus.Setting.Shared;
using Newtonsoft.Json;

namespace Fanex.ProductStatus.Setting.Models
{
    public class ProductStatusCommand
    {
        public int SportType { get; set; }

        public int BetType { get; set; }

        [JsonProperty("IsDisabled")]
        public bool Disable { get; set; }

        public bool UpdateAllMasters { get; set; }

        public bool DisableAllMasters { get; set; }

        public bool UpdateAllAgents { get; set; }

        public bool DisableAllAgents { get; set; }

        public bool UpdateAllMembers { get; set; }

        public bool DisableAllMembers { get; set; }

        public bool UpdateAllSuperDirectMembers { get; set; }

        public bool DisableAllSuperDirectMembers { get; set; }

        public bool UpdateAllMasterDirectMembers { get; set; }

        public bool DisableAllMasterDirectMembers { get; set; }

        public ProductStatusCommand CorrectProperties(short roleId)
        {
            if (Disable)
            {
                if (roleId == RoleIdentity.Super)
                {
                    UpdateAllMasters = true;
                    DisableAllMasters = true;
                    UpdateAllSuperDirectMembers = true;
                    DisableAllSuperDirectMembers = true;
                    --roleId;
                }

                if (roleId == RoleIdentity.Master)
                {
                    UpdateAllAgents = true;
                    DisableAllAgents = true;
                    UpdateAllMasterDirectMembers = true;
                    DisableAllMasterDirectMembers = true;
                    --roleId;
                }

                if (roleId == RoleIdentity.Agent)
                {
                    UpdateAllMembers = true;
                    DisableAllMembers = true;
                }
            }

            return this;
        }
    }
}