// -----------------------------------------------------------------------
// <copyright file="UserModel.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.ViewModel.Customer
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Extension;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerModel
    {
        [Required]
        [EASDisplay("Firstname", typeof(ViewLang))]
        public string FirstName { get; set; }

        [EASDisplay("Lastname", typeof(ViewLang))]
        public string LastName { get; set; }

        [Required]
        [EASDisplay("HomePhone", typeof(ViewLang))]
        public string HomePhone { get; set; }

        [EASDisplay("MobilePhone", typeof(ViewLang))]
        public string MobilePhone { get; set; }

        [Required]
        [EASDisplay("Email", typeof(ViewLang))]
        public string Email { get; set; }

        [EASDisplay("Fax", typeof(ViewLang))]
        public string Fax { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [EASDisplay("Password", typeof(ViewLang))]
        public string Password { get; set; }
    }
}