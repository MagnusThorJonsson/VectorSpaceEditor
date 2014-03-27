using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.MapData.Components
{
    /// <summary>
    /// User Item Property container
    /// </summary>
    [DataContract]
    public class ItemProperty
    {
        /// <summary>
        /// The property key
        /// </summary>
        [DataMember(Order = 0)]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        private string key;


        /// <summary>
        /// The property value
        /// </summary>
        [DataMember(Order = 1)]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        private string value;

        /// <summary>
        /// Constructs an item property
        /// </summary>
        /// <param name="key">The property key</param>
        /// <param name="value">The property value</param>
        public ItemProperty(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
