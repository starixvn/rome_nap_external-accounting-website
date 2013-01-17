// -----------------------------------------------------------------------
// <copyright file="LoginModel.cs" company="">
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
    public class LoginModel
    {
        [EASDisplay("LoginError", typeof(ViewLang))]
        public string LoginError
        {
            get
            {
                return ViewLang.LoginError;
            }
        }

        [EASDisplay("Login", typeof(ViewLang))]
        public string Login
        {
            get
            {
                return ViewLang.Login;
            }
        }

        [EASDisplay("LoginInfo", typeof(ViewLang))]
        public string Info
        {
            get
            {
                return ViewLang.LoginInfo;
            }
        }

        [EASDisplay("LoginTitle", typeof(ViewLang))]
        public string LoginTitle
        {
            get
            {
                return ViewLang.LoginTitle;
            }
        }

        [DataType(DataType.Password)]
        [EASDisplay("Confirmnewpassword", typeof(ViewLang))]
        public string ConfirmNewPassword { get; set; }

        [DataType(DataType.Password)]
        [EASDisplay("Confirmpassword", typeof(ViewLang))]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [EASDisplay("Newpassword", typeof(ViewLang))]
        public string NewPassword { get; set; }
    }
}