using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public static class Extensions
    {
        public static int ToUnixTimestamp(this DateTime date) {
            return (int)(date - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime ToDateTime(this int timestamp) {
            return new DateTime(1970, 1, 1) + new TimeSpan(0, 0, timestamp);
        }

        public static TEnum ParseToEnum<TEnum>(this string input) {
            var camelCased = input.Split('_').Aggregate("", (sum, item) => sum += item[0].ToString().ToUpper() + item.Substring(1));
            if (Enum.IsDefined(typeof(TEnum), camelCased)) return (TEnum)Enum.Parse(typeof(TEnum), camelCased);

            var unicased = input.Replace("_", "").ToLower();
            var match = Enum.GetNames(typeof(TEnum)).FirstOrDefault(name => name.ToLower() == unicased);
            
            if (match != null) return (TEnum)Enum.Parse(typeof(TEnum), match);

            throw new Exception("No match found in specified enum!");
        }

        public static bool HasProperty(this JToken token, string key) {
            return token.Children().FirstOrDefault(child => (child is JProperty) && ((JProperty)child).Name == key) != null;
        }
    }
}
