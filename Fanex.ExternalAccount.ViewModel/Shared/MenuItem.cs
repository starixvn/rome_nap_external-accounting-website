namespace Fanex.ExternalAccount.ViewModel.Shared
{
    using System.Collections.Generic;

    public class MenuItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<MenuItem> Children { get; set; }
    }
}