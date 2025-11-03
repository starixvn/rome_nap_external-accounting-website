namespace Fanex.ExternalAccount.DTO.Customer.Status
{
    using Core.Shared;

    public class SingleOpenAffectedDownline
    {
        public SingleOpenAffectedDownline(int level, int affectedDownlines, int remainUnchangedDownlines)
        {
            Level = level;
            AffectedDownlines = affectedDownlines;
            RemainUnchangedDownlines = remainUnchangedDownlines;
        }

        public int Level { get; }

        public string LevelName
        {
            get
            {
                switch (Level)
                {
                    case Role.Member:
                    case Role.Agent:
                    case Role.Master:
                    case Role.Super:
                        return Role.DisplayRoleName(Level);

                    default:
                        return "Total";
                }
            }
        }

        public int AffectedDownlines { get; }

        public int RemainUnchangedDownlines { get; }
    }
}