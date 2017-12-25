namespace Fanex.ExternalAccount.ViewModel.Extension
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public class EasDisplayAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo _nameProperty;

        public EasDisplayAttribute(string displayNameKey, Type resourceType)
            : base(displayNameKey)
        {
            if (resourceType != null)
            {
                this._nameProperty = resourceType.GetProperty(base.DisplayName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this._nameProperty == null)
                {
                    return base.DisplayName;
                }

                return (string)this._nameProperty.GetValue(this._nameProperty.DeclaringType, null);
            }
        }
    }
}