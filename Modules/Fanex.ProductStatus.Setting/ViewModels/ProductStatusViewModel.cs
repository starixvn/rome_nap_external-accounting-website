namespace Fanex.ProductStatus.Setting.ViewModels
{
    using Models;
    using Shared;

    public class ProductStatusViewModel
    {
        public ProductStatusViewModel(ProductStatusModel model, short custRole)
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
                    DirectSuperMember = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledSuperDirectMembers, model.TotalEnabledSuperDirectMembers);
                    Master = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledMasters, model.TotalEnabledMasters);
                    DirectMasterMember = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledMasters == 0, model.TotalDisabledMasterDirectMembers, model.TotalEnabledMasterDirectMembers);
                    Agent = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledMasters == 0, model.TotalDisabledAgents, model.TotalEnabledAgents);
                    Member = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledAgents == 0, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                case RoleIdentity.Master:
                    DirectMasterMember = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledMasterDirectMembers, model.TotalEnabledMasterDirectMembers);
                    Agent = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledAgents, model.TotalEnabledAgents);
                    Member = new DownlineStatusViewModel(IsDisabled || model.TotalEnabledAgents == 0, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                case RoleIdentity.Agent:
                    Member = new DownlineStatusViewModel(IsDisabled, model.TotalDisabledMembers, model.TotalEnabledMembers);
                    break;

                default:
                    DirectSuperMember = null;
                    Master = null;
                    DirectMasterMember = null;
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

        public DownlineStatusViewModel DirectSuperMember { get; }

        public bool IsShowDirectSuperMember
            => DirectSuperMember != null && DirectSuperMember.TotalDisabled + DirectSuperMember.TotalEnabled > 0;

        public DownlineStatusViewModel Master { get; }

        public bool IsShowMaster
            => Master != null && Master.TotalDisabled + Master.TotalEnabled > 0;

        public DownlineStatusViewModel DirectMasterMember { get; }

        public bool IsShowDirectMasterMember
            => DirectMasterMember != null && DirectMasterMember.TotalDisabled + DirectMasterMember.TotalEnabled > 0;

        public DownlineStatusViewModel Agent { get; }

        public bool IsShowAgent
            => Agent != null && Agent.TotalDisabled + Agent.TotalEnabled > 0;

        public DownlineStatusViewModel Member { get; }

        public bool IsShowMember
            => Member != null && Member.TotalDisabled + Member.TotalEnabled > 0;
    }
}