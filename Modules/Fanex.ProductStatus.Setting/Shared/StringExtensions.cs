namespace Fanex.ProductStatus.Setting.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class StringExtensions
    {
        public static IDictionary<int, bool> ToDictionary(this string value)
        {
            IDictionary<int, bool> dictionary = new Dictionary<int, bool>();

            int intValue;
            var stringArray = value.SplitString();

            foreach (var item in stringArray)
            {
                if (int.TryParse(item, out intValue))
                {
                    dictionary[intValue] = true;
                }
            }

            return dictionary;
        }

        public static int[] SplitInt(this string value)
        {
            try
            {
                return value
                    .SplitString()
                    .Select(item => int.Parse(item))
                    .ToArray();
            }
            catch (Exception)
            {
                return new int[0];
            }
        }

        public static string[] SplitString(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? new string[0]
                : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
