using System.Configuration;

namespace Fanex.ExternalAccount.Core.Configurations.SecurityCode
{
    public sealed class SecurityCodeSection : ConfigurationSection
    {
        [ConfigurationProperty("ActionFilterCollection", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ActionFilterCollection), AddItemName = "add")]
        public ActionFilterCollection ActionFilterCollection => (ActionFilterCollection)base["ActionFilterCollection"];

        [ConfigurationProperty("UserCodeCollection", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(UserCodeCollection), AddItemName = "add")]
        public UserCodeCollection UserCodeCollection => (UserCodeCollection)base["UserCodeCollection"];
    }

    public sealed class ActionFilterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new ActionFilterElement();

        protected override object GetElementKey(ConfigurationElement element) => ((ActionFilterElement)element).Action;

        new public UserCodeElement this[string name]
        {
            get
            {
                return (UserCodeElement)BaseGet(name);
            }
        }
    }

    public sealed class UserCodeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new UserCodeElement();

        protected override object GetElementKey(ConfigurationElement element) => ((UserCodeElement)element).UserName;

        new public UserCodeElement this[string name]
        {
            get
            {
                return (UserCodeElement)BaseGet(name);
            }
        }
    }

    public sealed class ActionFilterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("action", IsRequired = true)]
        public string Action
        {
            get
            {
                return this["action"].ToString();
            }
            set
            {
                this["action"] = value;
            }
        }
    }

    public sealed class UserCodeElement : ConfigurationElement
    {
        [ConfigurationProperty("user", IsRequired = true, IsKey = true)]
        public string UserName
        {
            get
            {
                return this["user"].ToString();
            }
            set
            {
                this["user"] = value;
            }
        }

        [ConfigurationProperty("code", IsRequired = true)]
        public string SecurityCode
        {
            get
            {
                return this["code"].ToString();
            }
            set
            {
                this["code"] = value;
            }
        }
    }
}