namespace Fanex.ProductStatus.Setting.Configuration
{
    public class Product
    {
        public Product(int id, string name, bool isGroup)
        {
            Id = id;
            Name = name;
            IsGroup = isGroup;
        }

        public int Id { get; }

        public string Name { get; }

        public bool IsGroup { get; }
    }
}