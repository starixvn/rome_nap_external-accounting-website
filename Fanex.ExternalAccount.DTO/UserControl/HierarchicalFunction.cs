namespace Fanex.ExternalAccount.DTO.UserControl
{
    using System.Collections.Generic;

    public class HierarchicalFunction
    {
        public HierarchicalFunction(int id, string name)
        {
            Id = id;
            Name = name;
            SubFunctions = new List<HierarchicalFunction>();
        }

        public int Id { get; }

        public string Name { get; }

        public IList<HierarchicalFunction> SubFunctions { get; set; }

        public bool HasSubFunctions => SubFunctions.Count > 0;
    }
}
