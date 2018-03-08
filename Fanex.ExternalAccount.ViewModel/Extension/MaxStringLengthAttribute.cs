namespace Fanex.ExternalAccount.ViewModel.Extension
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MaxStringLengthAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' must be under {1} characters.";
        private readonly int _maxCharacters = 50;

        public MaxStringLengthAttribute(int maxCharacters)
            : base(DefaultErrorMessage)
        {
            this._maxCharacters = maxCharacters;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, this.ErrorMessageString, name, this._maxCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return valueAsString != null && valueAsString.Length < this._maxCharacters;
        }
    }
}