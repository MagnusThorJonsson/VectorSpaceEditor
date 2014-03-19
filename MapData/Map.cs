using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                new Layer(_nextLayerId.ToString())
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
                new Layer(_nextLayerId.ToString(), name)
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
                return true;
            }

            return false;
        }
        #endregion


        #region Item Methods
        /// <summary>
        /// Adds an item to the specified layer
        /// </summary>
        /// <param name="layer">The layer index</param>
        /// <param name="item">The item to add to the layer</param>
        public void AddItem(int layer, IRenderable item)
        {
            if (layer < _layers.Count)
            {
                // Do we need this cross-coupling?
                item.Layer = layer;

                // Add to both the specified layer and mapitem list
                _layers[layer].AddItem(item);
                _mapItems.Add(item);
            }
        }

        public IRenderable CreateItem(int layer, string name, Texture texture, WorldPosition position)
        {
            //_mapItems.Single(i => i.ZIndex == 1);
            /*
            IOrderedEnumerable<IRenderable> bla = from item in _mapItems 
                                                  orderby item.ZIndex 
                                                  select item;
            */
            int zIndex = 0;
            // TODO: Make the layer id a string lookup
            TextureItem textureItem = TextureItem.Create(
                layer, 
                name, 
                texture, 
                position,
                zIndex
            );
            this.AddItem(layer, textureItem);

            return textureItem;
        }
        #endregion

    }
}
