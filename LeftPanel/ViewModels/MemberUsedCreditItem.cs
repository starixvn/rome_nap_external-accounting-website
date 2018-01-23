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

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Amount { get; set; }

        public bool IsClosed { get; set; }
    }
}