using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Components;

namespace VectorSpace.MapData.Interfaces
{
    /// <summary>
    /// Used by objects that contain user properties
    /// </summary>
    public interface IHasProperties
    {
        #region Properties
        /// <summary>
        /// Property holder name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Texture user properties
        /// </summary>
        ObservableCollection<ItemProperty> Properties { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Adds a property to the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        void AddProperty(string key, string value);

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>True on success</returns>
        bool RemoveProperty(ItemProperty property);

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="property">The property to remove</param>
        /// <returns>True on success</returns>
        bool RemoveProperty(string key);
        #endregion
    }
}
