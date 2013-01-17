// -----------------------------------------------------------------------
// <copyright file="DisplayAttribute.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
        private readonly PropertyInfo nameProperty;

        public EASDisplayAttribute(string displayNameKey, Type resourceType)
            : base(displayNameKey)
        {
            if (resourceType != null)
            {
                this.nameProperty = resourceType.GetProperty(base.DisplayName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.nameProperty == null)
                {
                    return base.DisplayName;
                }

                return (string)this.nameProperty.GetValue(this.nameProperty.DeclaringType, null);
            }
        }
    }
}