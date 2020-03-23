namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System;

    public class ObjectInfo
    {
        public ObjectInfo()
        {
        }

        public ObjectInfo(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Key = key;
            Value = value;
        }

        public ObjectInfo(string key, object value, string text) : this(key, value)
        {
            Text = text;
        }

        public ObjectInfo(string key, object value, string text, string tooltip) : this(key, value, text)
        {
            Tooltip = tooltip;
        }

        public ObjectInfo(string key, object value, string text, string tooltip, bool disabled)
            : this(key, value, text, tooltip)
        {
            Disabled = disabled;
        }

        public string Key { get; set; }

        public object Value { get; set; }

        public string Text { get; set; }

        public string Tooltip { get; set; }

        public bool Disabled { get; set; }

        public bool Visibled { get; set; } = true;

        public static ObjectInfo CreateInstance(string text, object value)
            => new ObjectInfo { Value = value, Text = text };
    }
}