namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    using System.Configuration;

    public sealed class SecurityCodeSection : ConfigurationSection
    {
        [ConfigurationProperty("ActionFilterCollection", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ActionFilterCollection), AddItemName = "add")]
        public ActionFilterCollection ActionFilterCollection => (ActionFilterCollection)base["ActionFilterCollection"];

        [ConfigurationProperty("UserCodeCollection", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(UserCodeCollection), AddItemName = "add")]
        public UserCodeCollection UserCodeCollection => (UserCodeCollection)base["UserCodeCollection"];

        [ConfigurationProperty("scAliveTime", IsRequired = true)]
        public string ScAliveTime
        {
            get => this["scAliveTime"].ToString();
            set => this["scAliveTime"] = value;
        }

        [ConfigurationProperty("scMaxAllowAttempt", IsRequired = true)]
        public int ScMaxAllowAttempt
        {
            get => (int)this["scMaxAllowAttempt"];
            set => this["scMaxAllowAttempt"] = value;
        }
    }

    public sealed class ActionFilterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new ActionFilterElement();

        protected override object GetElementKey(ConfigurationElement element) => ((ActionFilterElement)element).Action;

        public new ActionFilterElement this[string name] => (ActionFilterElement)BaseGet(name);
    }

    public sealed class UserCodeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new UserCodeElement();

        protected override object GetElementKey(ConfigurationElement element) => ((UserCodeElement)element).UserName;

        public new UserCodeElement this[string name] => (UserCodeElement)BaseGet(name);
    }

    public sealed class ActionFilterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get => this["name"].ToString();
            set => this["name"] = value;
        }

        [ConfigurationProperty("action", IsRequired = true)]
        public string Action
        {
            get => this["action"].ToString();
            set => this["action"] = value;
        }

        [ConfigurationProperty("isIgnored", DefaultValue = false, IsRequired = false)]
        public bool IsIgnored
        {
            get => (bool)this["isIgnored"];
            set => this["isIgnored"] = value;
        }
    }

    public sealed class UserCodeElement : ConfigurationElement
    {
        [ConfigurationProperty("user", IsRequired = true, IsKey = true)]
        public string UserName
        {
            get => this["user"].ToString();
            set => this["user"] = value;
        }

        [ConfigurationProperty("code", IsRequired = true)]
        public string SecurityCode
        {
            get => this["code"].ToString();
            set => this["code"] = value;
        }
    }
}