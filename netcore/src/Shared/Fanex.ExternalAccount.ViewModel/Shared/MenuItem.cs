using System.Collections.Generic;

namespace Fanex.ExternalAccount.ViewModel.Shared
{
    public class MenuItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<MenuItem> Children { get; set; }
    }
}