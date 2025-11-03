namespace Fanex.ExternalAccount.DTO.UserControl
{
    using System.Collections.Generic;

    public class Function
    {
        public int FuncID { get; set; }

        public string FuncName { get; set; }

        public string FuncDesc { get; set; }

        public string FuncKey { get; set; }

        public int ParentID { get; set; }

        public string ParentName { get; set; }

        public string CreatedDate { get; set; }

        public int Creator { get; set; }

        public int Modifier { get; set; }

        public string ModifiedDate { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsTest { get; set; }

        public EPolicy Policy { get; set; }

        public EPolicy GroupPolicy { get; set; }

        public IEnumerable<FunctionPolicy> AssignGroups { get; set; }
    }
}