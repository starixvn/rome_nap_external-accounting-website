namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System.ComponentModel.DataAnnotations;
    using Extension;
    using Utilities;

    public class CustomerModel
    {
        [Required]
        [EasDisplay("Firstname", typeof(ViewLang))]
        public string FirstName { get; set; }

        [EasDisplay("Lastname", typeof(ViewLang))]
        public string LastName { get; set; }

        [Required]
        [EasDisplay("Phone", typeof(ViewLang))]
        public string Phone { get; set; }

        [EasDisplay("MobilePhone", typeof(ViewLang))]
        public string MobilePhone { get; set; }

        [Required]
        [EasDisplay("Email", typeof(ViewLang))]
        public string Email { get; set; }

        [EasDisplay("Fax", typeof(ViewLang))]
        public string Fax { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [EasDisplay("Password", typeof(ViewLang))]
        public string Password { get; set; }
    }
}