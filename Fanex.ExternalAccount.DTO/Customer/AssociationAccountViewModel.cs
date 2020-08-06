namespace Fanex.ExternalAccount.DTO.Customer
{
    using System.Collections.Generic;

    public class AssociationAccountViewModel
    {
        public AssociationAccountViewModel()
        {
            Accounts = new List<AssociationAccountModel>();
        }

        public int PageIndex { get; set; }

        public int TotalAccount { get; set; }

        public int TotalRows { get; set; }

        public List<AssociationAccountModel> Accounts { get; set; }

        public string AssociationAccountText => $"{TotalRows}/{TotalAccount}";
    }
}
