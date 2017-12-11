namespace Fanex.ExternalAccount.ViewModel.ManageFunction
{
    public class CreateFunctionModel
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }
    }
}