namespace Fanex.ExternalAccount.ViewModel.Extension
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' and '{1}' do not match.";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(DefaultErrorMessage)
        {
            this.OriginalProperty = originalProperty;
            this.ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }

        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return this._typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            // temp hard code here
            return string.Format(CultureInfo.CurrentUICulture, this.ErrorMessageString, ViewLang.Newpassword, ViewLang.Confirmpassword);

            // return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
            //    OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(this.OriginalProperty, true).GetValue(value);
            object confirmValue = properties.Find(this.ConfirmProperty, true).GetValue(value);
            return object.Equals(originalValue, confirmValue);
        }
    }
}