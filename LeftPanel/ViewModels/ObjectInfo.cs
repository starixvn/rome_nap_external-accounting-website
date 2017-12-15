namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class ObjectInfo
    {
        public ObjectInfo()
        {
        }

        public ObjectInfo(string key, object value)
        {
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
    }
}
