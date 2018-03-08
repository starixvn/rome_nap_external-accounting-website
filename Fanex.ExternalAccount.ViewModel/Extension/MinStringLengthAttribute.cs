namespace Fanex.ExternalAccount.ViewModel.Extension
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MinStringLengthAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' must be at least {1} characters long.";
        private readonly int _minCharacters = 6;

        public MinStringLengthAttribute(int minCharacters)
            : base(DefaultErrorMessage)
        {
            this._minCharacters = minCharacters;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, this.ErrorMessageString, name, this._minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return valueAsString != null && valueAsString.Length >= this._minCharacters;
        }
    }
}