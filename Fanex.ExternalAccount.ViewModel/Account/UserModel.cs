// -----------------------------------------------------------------------
// <copyright file="UserModel.cs" company="Nexcel Solution Co.Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.ViewModel.Account
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Extension;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserModel
    {
        [Required]
        [EASDisplay("Username", typeof(ViewLang))]
        public string Username { get; set; }

        [Required]
        [EASDisplay("Groupname", typeof(ViewLang))]
        public string GroupName { get; set; }

        [Required]
        [EASDisplay("Firstname", typeof(ViewLang))]
        public string FirstName { get; set; }

        [EASDisplay("Lastname", typeof(ViewLang))]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [EASDisplay("Password", typeof(ViewLang))]
        public string Password { get; set; }
    }
}