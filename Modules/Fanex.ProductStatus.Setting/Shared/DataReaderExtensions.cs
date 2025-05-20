namespace Fanex.ProductStatus.Setting.Shared
{
    using System;
    using System.Data;

    internal static class DataReaderExtensions
    {
        public static bool HasColumn(this IDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static int ToInt32(object value)
        {
            var val = 0;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return 0;
            }

            if (value is decimal || value is double)
            {
                val = Convert.ToInt32(value);
            }
            else
            {
                int.TryParse(value.ToString(), out val);
            }

            return val;
        }
    }
}
