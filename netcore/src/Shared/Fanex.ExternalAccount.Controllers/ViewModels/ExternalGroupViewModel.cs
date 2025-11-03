namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    using System.Collections.Generic;
    using DTO.UserControl;

    public class ExternalGroupViewModel
    {
        public string Username { get; set; }

        public List<Group> GroupList { get; set; }

        public bool EnabledOTP { get; set; }

        public string Code { get; set; }

        public string ModifiedDate { get; set; }
    }
}