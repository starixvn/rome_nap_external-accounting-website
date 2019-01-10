namespace Fanex.ExternalAccount.Core.Configurations.InternalLogin
{
    using System.Configuration;

    public sealed class InternalUrlSection : ConfigurationSection
    {
        [ConfigurationProperty("InternalUrlCollection", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(InternalUrlCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public InternalUrlCollection InternalUrlCollection
            => (InternalUrlCollection)base["InternalUrlCollection"];
    }

    public sealed class InternalUrlCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
            => new InternalUrlElement();

        protected override object GetElementKey(ConfigurationElement element)
            => ((InternalUrlElement)element).Name;

        new public InternalUrlElement this[string name]
        {
            get
            {
                return (InternalUrlElement)base.BaseGet(name);
            }
        }
    }

    public sealed class InternalUrlElement : ConfigurationElement
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

        [ConfigurationProperty("isdefault", DefaultValue = false)]
        public bool IsDefault
        {
            get
            {
                return (bool)this["isdefault"];
            }
            set
            {
                this["isdefault"] = value;
            }
        }

        [ConfigurationProperty("sites", IsRequired = true)]
        public string Sites
        {
            get
            {
                return this["sites"].ToString();
            }
            set
            {
                this["sites"] = value;
            }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get
            {
                return this["url"].ToString();
            }
            set
            {
                this["url"] = value;
            }
        }
    }
}
