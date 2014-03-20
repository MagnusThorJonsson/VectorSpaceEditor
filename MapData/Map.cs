using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;
using VectorSpace.MapData.MapItems;

namespace VectorSpace.MapData
{
    public class Map : IHasProperties
    {
        #region Variables & Properties
        /// <summary>
        /// Map name
        /// </summary>
        public string Name { get { return _name; } }
        private string _name;

        /// <summary>
        /// The path to the map root directory
        /// </summary>
        public string FilePath { get { return _filepath; } }
        private string _filepath;

        /// <summary>
        /// The map width & height
        /// </summary>
        public Point Size { get { return _size; } }
        private Point _size;

        /// <summary>
        /// Map layers
        /// </summary>
        public ObservableCollection<Layer> Layers { get { return _layers; } }
        private ObservableCollection<Layer> _layers;
 
        // TODO: Shitty way, make custom observablecollection for this shiz
        /// <summary>
        /// The next layer id that is available
        /// </summary>
        public int NextLayerId { get { return _nextLayerId; } }
        private int _nextLayerId;

        /// <summary>
        /// User properties for the map
        /// </summary>
        public ObservableCollection<ItemProperty> Properties { get { return _properties; } }
        private ObservableCollection<ItemProperty> _properties;

        /// <summary>
        /// Texture libraries in use by the map
        /// </summary>
        public ObservableCollection<TextureLibrary> TextureLibraries { get { return _textureLibraries; } }
        private ObservableCollection<TextureLibrary> _textureLibraries;

        /// <summary>
        /// The list of items placed on the map
        /// </summary>
        public ObservableCollection<IRenderable> MapItems { get { return _mapItems; } }
        private ObservableCollection<IRenderable> _mapItems;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an empty map
        /// </summary>
        public Map()
        {
            _name = "New map";
            _filepath = null;
            _size = new Point(32, 32);

            _nextLayerId = 0;
            _layers = new ObservableCollection<Layer>();
            _properties = new ObservableCollection<ItemProperty>();
            _textureLibraries = new ObservableCollection<TextureLibrary>();
            _mapItems = new ObservableCollection<IRenderable>();

            // Create the first layer
            AddLayer("Layer");
        }

        /// <summary>
        /// Creates a named empty map
        /// </summary>
        /// <param name="name">The map name</param>
        /// <param name="path">The root directory path</param>
        public Map(string name, string path)
        {
            _name = name;
            _filepath = path;
            _size = new Point(32, 32);

            _nextLayerId = 0;
            _layers = new ObservableCollection<Layer>();
            _textureLibraries = new ObservableCollection<TextureLibrary>();
            _properties = new ObservableCollection<ItemProperty>();
            _mapItems = new ObservableCollection<IRenderable>();

            // Create the first layer
            AddLayer("Layer");
        }
        #endregion


        #region Initialization Methods
        /// <summary>
        /// Sets the map name
        /// </summary>
        /// <param name="name">The name for the map</param>
        public void SetName(string name)
        {
            if (name.Length > 0)
                _name = name;
        }

        /// <summary>
        /// Sets the map path
        /// </summary>
        /// <param name="path">The root directory path for the map</param>
        public void SetPath(string path)
        {
            if (path.Length > 0)
                _filepath = path;
        }

        /// <summary>
        /// Sets the map width and height
        /// </summary>
        /// <param name="width">Map width in pixels</param>
        /// <param name="height">Map with in pixels</param>
        public void SetSize(int width, int height)
        {
            _size = new Point(width, height);
        }
        #endregion


        #region Texture Library Methods
        /// <summary>
        /// Adds a new texture library
        /// </summary>
        /// <param name="library">The library to add</param>
        /// <returns>True if successful</returns>
        public bool AddTextureLibrary(TextureLibrary library)
        {
            for (int i = 0; i < _textureLibraries.Count; i++)
            {
                if (_textureLibraries[i] == library || _textureLibraries[i].Name == library.Name)
                    return false;
            }

            _textureLibraries.Add(library);

            return true;
        }

        /// <summary>
        /// Deletes a texture library
        /// </summary>
        /// <param name="library">The library to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveTextureLibrary(TextureLibrary library)
        {
            return _textureLibraries.Remove(library);
        }

        /// <summary>
        /// Deletes a texture library
        /// </summary>
        /// <param name="name">The name of the library to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveTextureLibrary(string name)
        {
            for (int i = 0; i < _textureLibraries.Count; i++)
            {
                if (_textureLibraries[i].Name.Equals(name))
                {
                    _textureLibraries.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
        #endregion


        #region User Property Handlers
        /// <summary>
        /// Adds a property to the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public void AddProperty(string key, string value)
        {
            Properties.Add(new ItemProperty(key, value));
        }

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>True on success</returns>
        public bool RemoveProperty(string key)
        {
            for (int i = 0; i < _properties.Count; i++)
            {
                if (_properties[i].Key.Equals(key))
                {
                    _properties.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="property">The property to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveProperty(ItemProperty property)
        {
            return Properties.Remove(property);
        }
        #endregion


        #region Layer Methods
        // TODO: Create a custom ObservableClass for the Layers and move these methods there
        /// <summary>
        /// Adds a layer to the map
        /// </summary>
        public void AddLayer()
        {
            // TODO: Generate unique id
            _layers.Add(
                new Layer(_nextLayerId)
            );
            _nextLayerId++;
        }


        /// <summary>
        /// Adds a layer to the map
        /// </summary>
        /// <param name="name">The layer name</param>
        public void AddLayer(string name)
        {
            // TODO: Generate unique id
            _layers.Add(
                new Layer(_nextLayerId, name)
            );
            _nextLayerId++;
        }

        /// <summary>
        /// Adds a layer to the map
        /// </summary>
        /// <param name="layer">The layer to add</param>
        public void AddLayer(Layer layer)
        {
            if (layer != null)
            {
                _layers.Add(layer);
                _nextLayerId++;
            }
        }

        /// <summary>
        /// Removes a layer from the map
        /// </summary>
        /// <param name="layer">The layer to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveLayer(Layer layer)
        {
            if (layer != null)
            {
                // Remove all layer items from the mapItems array
                for (int i = 0; i < layer.Items.Count; i++)
                    _mapItems.Remove(layer.Items[i]);

                // Clear all items from the layer
                layer.Items.Clear();
                _layers.Remove(layer);

                // Close any ZIndex gaps that might have been made
                closeGapsZ();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a layer from the map
        /// </summary>
        /// <param name="index">The layer index to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveLayer(int index)
        {
            if (index < _layers.Count)
            {
                // Remove all layer items from the mapItems array
                for (int i = 0; i < _layers[index].Items.Count; i++)
                    _mapItems.Remove(_layers[index].Items[i]);

                // Clear all items from the layer
                _layers[index].Items.Clear();
                _layers.RemoveAt(index);

                // Close any ZIndex gaps that might have been made
                closeGapsZ();
                return true;
            }

            return false;
        }

        // TODO: This is a pretty bad way to handle this stuff, will get exponentially slower by the amount of items in the Layers
        /// <summary>
        /// Sorts the layer items by ZIndex in descending order
        /// </summary>
        private void _sortLayerItems()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].Items.Sort(x => x.ZIndex, ListSortDirection.Descending);
            }
        }
        #endregion


        #region Item Methods

        #region Item Add & Create Methods
        /// <summary>
        /// Adds an item to the specified layer
        /// </summary>
        /// <param name="layer">The layer index</param>
        /// <param name="item">The item to add to the layer</param>
        public void AddItem(int layer, IRenderable item)
        {
            // We need to get the zIndex before the item is added
            int zIndex = GetHighestZ(layer) + 1;

            if (layer < _layers.Count)
            {
                // Do we need this cross-coupling?
                item.Layer = layer;

                // Add to both the specified layer and mapitem list
                _layers[layer].AddItem(item);
                _mapItems.Add(item);
            }

            // Sets the zIndex for the newly added item
            SetItemZ(
                item,
                zIndex
            );

            _sortLayerItems();
        }

        /// <summary>
        /// Creates a new Texture Item on the Canvas map
        /// </summary>
        /// <param name="layer">The layer put the object on</param>
        /// <param name="name">The name of the object</param>
        /// <param name="texture">The texture for the item</param>
        /// <param name="position">The position</param>
        /// <returns>The created item</returns>
        public IRenderable CreateItem(int layer, string name, Texture texture, WorldPosition position)
        {
            // TODO: Make the layer id a string lookup
            TextureItem textureItem = TextureItem.Create(
                layer, 
                name, 
                texture, 
                position,
                0
            );
            this.AddItem(layer, textureItem);

            return textureItem;
        }
        #endregion

        #region Z Index Helpers
        protected void closeGapsZ()
        {
            List<IRenderable> sortedItems = _mapItems.OrderBy(x => x.ZIndex).ToList();

            int lastZ = -1;
            for (int i = 0; i < sortedItems.Count; i++)
            {
                // Adjust ZIndex until the gap is closed
                while (sortedItems[i].ZIndex > (lastZ + 1))
                    sortedItems[i].ZIndex -= 1;

                // Save the last ZIndex
                lastZ = sortedItems[i].ZIndex;
            }

            _sortLayerItems();
        }

        /// <summary>
        /// Sets the items Z Index 
        /// </summary>
        /// <param name="item">The item to change</param>
        /// <param name="zIndex">The new zIndex</param>
        public void SetItemZ(IRenderable item, int zIndex)
        {
            // Move all items that are higher or equal to the zIndex up by one 
            for (int i = 0; i < _mapItems.Count; i++)
            {
                if (_mapItems[i].ZIndex >= zIndex)
                    _mapItems[i].ZIndex += 1;
            }

            // Update item
            //IRenderable itemFound = _mapItems.Where(X => X == item).FirstOrDefault();
            item.ZIndex = zIndex;

            _sortLayerItems();
        }

        /// <summary>
        /// Brings an items Z-Index forward by one
        /// </summary>
        /// <param name="item">The item to bring forward</param>
        /// <param name="doSort">Flags whether to do layer sort (defaults to true)</param>
        /// <returns>True on success</returns>
        public bool IncrementItemZ(IRenderable item, bool doSort = true)
        {
            for (int i = 0; i < _mapItems.Count; i++)
            {
                if (_mapItems[i].Layer == item.Layer && _mapItems[i].ZIndex == (item.ZIndex + 1))
                {
                    _mapItems[i].ZIndex = item.ZIndex;
                    item.ZIndex += 1;
                    
                    if (doSort)
                        _sortLayerItems();
    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves an items Z-Index backwards by one
        /// </summary>
        /// <param name="item">The item to move backward</param>
        /// <param name="doSort">Flags whether to do layer sort (defaults to true)</param>
        /// <returns>True on success</returns>
        public bool DecrementItemZ(IRenderable item, bool doSort = true)
        {
            // If we're at the bottom we break
            if (item.ZIndex == 0)
                return false;

            for (int i = 0; i < _mapItems.Count; i++)
            {
                if (_mapItems[i].Layer == item.Layer && _mapItems[i].ZIndex == (item.ZIndex - 1))
                {
                    _mapItems[i].ZIndex = item.ZIndex;
                    item.ZIndex -= 1;

                    if (doSort)
                        _sortLayerItems();
                    
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Brings an item to the Front
        /// </summary>
        /// <param name="item">The item to bring to front</param>
        public void BringToFront(IRenderable item)
        {
            bool doAgain = true;
            while (doAgain)
                doAgain = IncrementItemZ(item, false);

            _sortLayerItems();
        }

        /// <summary>
        /// Sends an item to the Back
        /// </summary>
        /// <param name="item">The item to send to back</param>
        public void SendToBack(IRenderable item)
        {
            bool doAgain = true;
            while (doAgain)
                doAgain = DecrementItemZ(item, false);

            _sortLayerItems();
        }

        /// <summary>
        /// Gets the highest ZIndex from the objects on a given layer
        /// </summary>
        /// <param name="layer">The layer to check from</param>
        /// <returns>The highest index found or -1 when nothing is found</returns>
        public int GetHighestZ(int layer)
        {
            if (layer < _layers.Count)
            {
                if (_mapItems.Count > 0)
                {
                    IRenderable mapItem = _mapItems.Where(x => x.Layer == layer).OrderByDescending(x => x.ZIndex).FirstOrDefault();
                    if (mapItem != null)
                    {
                        return mapItem.ZIndex;
                    }
                    else
                    {
                        // If there are no items on this layer we need to check the one below
                        if (layer >= 0)
                        {
                            return GetHighestZ(layer - 1);
                        }
                    }
                }
            }

            return -1;
        }
        #endregion

        #endregion

    }
}
