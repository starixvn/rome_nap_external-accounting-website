namespace Fanex.ExternalAccount.ViewModel.Extension
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EASDisplayAttribute : DisplayNameAttribute
    {
        private readonly PropertyInfo _nameProperty;

        public EASDisplayAttribute(string displayNameKey, Type resourceType)
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