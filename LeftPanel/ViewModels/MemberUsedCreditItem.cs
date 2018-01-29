namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Data;
    using Extensions;

    public class MemberUsedCreditItem
    {
        public MemberUsedCreditItem(IDataReader reader)
        {
            Username = reader.Username();
            FirstName = reader.FirstName();
            LastName = reader.LastName();
            Amount = reader.Credit();
            IsClosed = reader.IsClosed();
        }

        public string Username { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public decimal Amount { get; }

        public bool IsClosed { get; }
    }
}