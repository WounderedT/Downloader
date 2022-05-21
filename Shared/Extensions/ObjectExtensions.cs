using Newtonsoft.Json;
using System.Text;

namespace Shared.Extensions
{
    /// <summary>
    /// Type provides common object and generic object extension methods
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings() { Formatting = Formatting.Indented };

        /// <summary>
        /// Serializes object to JSON string representation
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>A JSON string representation of object <paramref name="obj"/></returns>
        public static String ToJsonString<T>(this T obj)
        {
            return obj.ToJsonString(_settings);
        }

        /// <summary>
        /// Serializes object to JSON string representation using provided <paramref name="jsonSerializerSettings"/>
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <param name="jsonSerializerSettings">JSON serialization settings</param>
        /// <returns>A JSON string representation of object <paramref name="obj"/></returns>
        public static String ToJsonString<T>(this T obj, JsonSerializerSettings jsonSerializerSettings)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        /// <summary>
        /// Converts System.String object to System.Byte array using UTF-8 encoding.
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <returns>A byte array containing the results of the <paramref name="str"/> encoding.</returns>
        public static Byte[] ToBytes(this String str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
