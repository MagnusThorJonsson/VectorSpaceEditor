using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VectorSpace.MapData.Components; 
using VectorSpace.MapData.Interfaces;
using VectorSpace.MapData.MapItems;

namespace VectorSpace.MapData
{
    public class Texture : IHasProperties
    {
        #region Variables & Properties
        /// <summary>
        /// Texture file name
        /// </summary>
        public string Name { get { return filename; } }
        protected string filename;

        /// <summary>
        /// Texture file path
        /// </summary>
        public string Path { get { return filepath; } }
        protected string filepath;
        
        /// <summary>
        /// Texture source image
        /// </summary>
        public BitmapImage Source { get { return _source; } }
        private BitmapImage _source;

        /// <summary>
        /// Texture size
        /// </summary>
        public Point Size { get { return _size; } }
        private Point _size;

        /// <summary>
        /// Texture origin point
        /// </summary>
        public Point Origin 
        { 
            get { return _origin; }
            set { _origin = value; }
        }
        private Point _origin;
        
        /// <summary>
        /// Texture user properties
        /// </summary>
        public ObservableCollection<ItemProperty> Properties { get { return properties; } }
        protected ObservableCollection<ItemProperty> properties;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructs a texture object
        /// </summary>
        /// <param name="filepath">The full file path</param>
        public Texture(string filepath)
        {
            filename = System.IO.Path.GetFileName(filepath);
            this.filepath = System.IO.Path.GetDirectoryName(filepath);
            
            _source = new BitmapImage();
            _source.BeginInit();
            _source.CacheOption = BitmapCacheOption.OnLoad;
            _source.UriSource = new Uri(filepath);
            _source.EndInit();

            _size = new Point((int)_source.Width, (int)_source.Height);
            _origin = new Point(_size.X / 2, _size.Y / 2);

            properties = new ObservableCollection<ItemProperty>();
        }

        /// <summary>
        /// Constructs a texture object
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="path">The file path</param>
        public Texture(string name, string path)
        {
            filename = name;
            filepath = path;

            _source = new BitmapImage();
            _source.BeginInit();
            _source.CacheOption = BitmapCacheOption.OnLoad;
            _source.UriSource = new Uri(path + "\\" + name);
            _source.EndInit();

            _size = new Point((int)_source.Width, (int)_source.Height);
            _origin = new Point(_size.X / 2, _size.Y / 2);

            properties = new ObservableCollection<ItemProperty>();
        }

        /// <summary>
        /// Constructs a texture object
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="path">The file path</param>
        /// <param name="source">The image source</param>
        public Texture(string name, string path, BitmapImage source)
        {
            filename = name;
            filepath = path;
            _source = source;

            _size = new Point((int)source.Width, (int)source.Height);
            _origin = new Point(_size.X / 2, _size.Y / 2);

            properties = new ObservableCollection<ItemProperty>();
        }
        
        /// <summary>
        /// Constructs a texture object
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="path">The file path</param>
        /// <param name="source">The image source</param>
        /// <param name="size">The image size</param>
        public Texture(string name, string path, BitmapImage source, Point size)
        {
            filename = name;
            filepath = path;
            _source = source;
            _size = size;

            _origin = new Point(_size.X / 2, _size.Y / 2);

            properties = new ObservableCollection<ItemProperty>();
        }

        /// <summary>
        /// Constructs a texture object
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="path">The file path</param>
        /// <param name="source">The image source</param>
        /// <param name="size">The image size</param>
        /// <param name="origin">The image origin point</param>
        public Texture(string name, string path, BitmapImage source, Point size, Point origin)
        {
            filename = name;
            filepath = path;
            _source = source;
            _size = size;
            _origin = origin;

            properties = new ObservableCollection<ItemProperty>();
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
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].Key.Equals(key))
                {
                    properties.RemoveAt(i);
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

    }
}
