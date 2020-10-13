namespace Fanex.ExternalAccount.SearchAndCopy.Requests
{
    public class SuperPrefixRequest
    {
        public string GroupName { get; set; }
        public string Prefix { get; set; }
        public bool? AllowNewSuper { get; set; }
    }
}