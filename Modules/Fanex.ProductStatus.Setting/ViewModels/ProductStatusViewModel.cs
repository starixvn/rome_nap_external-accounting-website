namespace Fanex.ProductStatus.Setting.ViewModels
{
    using Models;
    using Shared;

    public class ProductStatusViewModel
    {
        public ProductStatusViewModel(ProductStatusModel model, byte custRole)
        {
            Name = model.Name;
            SportType = model.SportType;
            ParentSportType = model.ParentSportType;
            BetType = model.BetType;
            ParentBetType = model.ParentBetType;
            IsDisabled = model.IsDisabledUpline || model.IsDisabled;
            IsDisabledUpline = model.IsDisabledUpline;

            switch (custRole)
            {
                case RoleIdentity.Super:
                    Master = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledMasters, model.TotalEnabledMasters);
                    Agent = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledMasters == 0, model.TotalDisabledAgents, model.TotalEnabledAgents);
                    Member = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledAgents == 0, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                case RoleIdentity.Master:
                    Agent = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledAgents, model.TotalEnabledAgents);
                    Member = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledAgents == 0, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                case RoleIdentity.Agent:
                    Member = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                default:
                    Master = null;
                    Agent = null;
                    Member = null;
                    break;
            }
        }

        public string Name { get; }

        public int SportType { get; }

        public int ParentSportType { get; }

        public int BetType { get; }

        public int ParentBetType { get; }

        public bool IsDisabled { get; }

        public bool IsDisabledUpline { get; }

        public DownlineStatusViewModel Master { get; }

        public bool IsShowMaster
            => Master != null && Master.TotalDisabled + Master.TotalEnabled > 0;

        public DownlineStatusViewModel Agent { get; }

        public bool IsShowAgent
            => Agent != null && Agent.TotalDisabled + Agent.TotalEnabled > 0;

        public DownlineStatusViewModel Member { get; }

        public bool IsShowMember
            => Member != null && Member.TotalDisabled + Member.TotalEnabled > 0;
    }
}