namespace Fanex.ExternalAccount.ViewModel.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// Menu Item object.
    /// </summary>
    public class MenuItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<MenuItem> Children { get; set; }
    }
}