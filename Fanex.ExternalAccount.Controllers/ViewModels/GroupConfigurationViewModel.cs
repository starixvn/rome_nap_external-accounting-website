namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    using System.Collections.Generic;
    using DTO.Products;
    using Fanex.ExternalAccount.DTO.UserControl;

    public class GroupConfigurationViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; }

        public IEnumerable<Group> Groups { get; set; }
    }
}
