// -----------------------------------------------------------------------
// <copyright file="Login.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.ViewModel.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Captcha { get; set; }

        public string Code { get; set; }
    }
}