namespace Fanex.ExternalAccount.ViewModel.ProductStatusSetting
{
    using System.Collections.Generic;

    public class MultipleViewModel
    {
        public int RoleId { get; set; }

        public int[] ProductIds { get; set; }

        public int[] CustIds { get; set; }

        public string[] CustNames { get; set; }

        public IEnumerable<KeyValuePair<int, string>> Products { get; set; }
    }
}
