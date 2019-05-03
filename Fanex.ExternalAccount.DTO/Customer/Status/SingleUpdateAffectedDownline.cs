namespace Fanex.ExternalAccount.DTO.Customer.Status
{
    using Core.Shared;

    public class SingleUpdateAffectedDownline
    {
        public SingleUpdateAffectedDownline(int level, int affectedDownlines, int remainUnchangedDownlines)
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
                    case 1:
                    case 2:
                    case 3:
                    case 4:
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
