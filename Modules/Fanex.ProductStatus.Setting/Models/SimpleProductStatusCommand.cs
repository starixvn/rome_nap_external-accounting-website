using Fanex.ProductStatus.Setting.Shared;

namespace Fanex.ProductStatus.Setting.Models
{
    public class SimpleProductStatusCommand
    {
        public int SportType { get; set; }

        public int BetType { get; set; }

        public bool Disable { get; set; }

        public bool? DisableDownlines { get; set; }

        public ProductStatusCommand ToProductStatusCommand(byte roleId)
        {
            var command = new ProductStatusCommand
            {
                SportType = SportType,
                BetType = BetType,
                Disable = Disable
            };

            if (Disable)
            {
                command = command.CorrectProperties(roleId);
            }
            else if (DisableDownlines.HasValue)
            {
                if (roleId == RoleIdentity.Super)
                {
                    command.UpdateAllMasters = true;
                    command.DisableAllMasters = DisableDownlines.Value;
                    --roleId;
                }

                if (roleId == RoleIdentity.Master)
                {
                    command.UpdateAllAgents = true;
                    command.DisableAllAgents = DisableDownlines.Value;
                    --roleId;
                }

                if (roleId == RoleIdentity.Agent)
                {
                    command.UpdateAllMembers = true;
                    command.DisableAllMembers = DisableDownlines.Value;
                }
            }

            return command;
        }
    }
}
