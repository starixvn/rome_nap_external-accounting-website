namespace Fanex.ExternalAccount.Statement.Models
{
    using System;

    /// <summary>
    /// Data Column Model.
    /// </summary>
    [Serializable]
    public class DataColumnModel
    {
        public bool HasData { get; set; }

        public object Value { get; set; }
    }
}
