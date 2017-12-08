namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Personal information object.
    /// </summary>
    public class PersonalInfo : DownlineList
    {
        public PersonalInfo()
            : base()
        {
            IsShowProducts = new List<ObjectInfo>();
        }

        public ObjectInfo CurrentLevel { get; set; }

        public ObjectInfo Name { get; set; }

        public ObjectInfo Nickname { get; set; }

        public ObjectInfo Phone { get; set; }

        public ObjectInfo Mobile { get; set; }

        public ObjectInfo Fax { get; set; }

        public ObjectInfo CustList { get; set; }

        public ObjectInfo Status { get; set; }

        public ObjectInfo BackgroundColor { get; set; }

        public ObjectInfo IsInternal { get; set; }

        public IList<ObjectInfo> IsShowProducts { get; set; }
    }
}
