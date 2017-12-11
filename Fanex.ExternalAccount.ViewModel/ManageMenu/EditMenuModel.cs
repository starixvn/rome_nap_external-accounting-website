namespace Fanex.ExternalAccount.ViewModel.ManageMenu
{
    public class EditMenuModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public int FunctionId { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public int DisplayOrder { get; set; }
    }
}