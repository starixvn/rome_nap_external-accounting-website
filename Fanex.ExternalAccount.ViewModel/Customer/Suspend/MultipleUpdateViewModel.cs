namespace Fanex.ExternalAccount.ViewModel.Customer.Suspend
{
    public class MultipleUpdateViewModel
    {
        public int CustId { get; set; }

        public int RoleId { get; set; }

        public string UserName { get; set; }

        public bool Suspend { get; set; }

        public int AffectedMaster { get; set; }

        public int AffectedAgent { get; set; }

        public int AffectedMember { get; set; }

        public int RemainUnchangedMaster { get; set; }

        public int RemainUnchangedAgent { get; set; }

        public int RemainUnchangedMember { get; set; }
    }
}
