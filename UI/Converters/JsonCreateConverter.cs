using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.UI.Converters
{
    /// <summary>
    /// Handles the creation of object types on load
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonCreateConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        /// <summary>
        /// Checks whether an object can be converted
        /// </summary>
        /// <param name="objectType">The object type</param>
        /// <returns>True if possible</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads the JSON from a string
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="objectType">The object type</param>
        /// <param name="existingValue">The existing value</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>The serialized content</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        /// <summary>
        /// Writes the JSON to a string
        /// </summary>
        /// <param name="writer">The writer</param>
        /// <param name="value">The value</param>
        /// <param name="serializer">The serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
            //throw new NotImplementedException(string.Format("Write action for {0} not implemented.", value.ToString()));
        }
    }
}
