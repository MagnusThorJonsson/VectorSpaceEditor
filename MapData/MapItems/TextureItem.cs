using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VectorSpace.UI.Converters;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData.MapItems
{
    /// <summary>
    /// Map Item that is a Texture/Sprite
    /// </summary>
    public class TextureItem : IRenderable, IHasProperties
    {
        #region Variables & Properties
        protected string type;

        protected string layer;
        protected string name;

        /// <summary>
        /// The texture this item uses
        /// </summary>
        [JsonIgnore]
        public Texture Texture 
        { 
            get { return texture; }
            set { texture = value; }
        }
        protected Texture texture;

        protected WorldPosition position;
        protected int zIndex;

        /// <summary>
        /// TextureItem user properties
        /// </summary>
        [JsonProperty(Order = 7)]
        public ObservableCollection<ItemProperty> Properties 
        { 
            get { return properties; }
            protected set { properties = value; }
        }
        protected ObservableCollection<ItemProperty> properties;

        /// <summary>
        /// Is Visible property
        /// </summary>
        [JsonIgnore]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged("IsVisible");
                OnIsVisibleChanged(EventArgs.Empty);
            }
        }
        protected bool isVisible;

        /// <summary>
        /// Is Selected property
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                OnIsSelectedChanged(EventArgs.Empty);
            }
        }
        protected bool isSelected;

        /// <summary>
        /// Current width based on scale
        /// </summary>
        [JsonIgnore]
        public float Width
        {
            get
            {
                return texture.Size.X * position.ScaleX;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleX = value / texture.Size.X;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Width");
                }
            }
        }

        /// <summary>
        /// Current height based on scale
        /// </summary>
        [JsonIgnore]
        public float Height
        {
            get
            {
                return texture.Size.Y * position.ScaleY;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleY = value / texture.Size.Y;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary>
        /// Current angle in degrees
        /// </summary>
        [JsonIgnore]
        public float Angle
        {
            get
            {
                return position.Rotation;
                /*
                // Radians to Degrees
                float degrees = (float)(180f / Math.PI * position.Rotation);
                
                if (degrees < 0f)
                    degrees += 360f;
                else if (degrees > 360f)
                    degrees -= 360f;
                return degrees;         
                */  
            }
            set
            {
                // Degrees to radians
                //position.Rotation = WrapRotation((float)((Math.PI / 180f) * value));
                position.Rotation = value;//(float)(Math.PI / 180f * value);

                OnPropertyChanged("Angle");
            }
        }

        #endregion


        #region Interface Properties
        /// <summary>
        /// Item type
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Type
        {
            get { return type; }
            protected set { type = value; }
        }

        /// <summary>
        /// Item name
        /// </summary>
        [JsonProperty(Order = 2)]
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// The name of the texture used by this item
        /// </summary>
        [JsonProperty(Order = 3)]
        public string TextureName
        {
            get 
            { 
                if (texture != null)
                    return texture.Name;

                return _textureName;
            }
            protected set 
            {
                if (texture == null)
                    _textureName = value;
                else
                    _textureName = texture.Name;
            }
        }
        private string _textureName;

        /// <summary>
        /// Item layer id
        /// </summary>
        [JsonProperty(Order = 4)]
        public string Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                OnPropertyChanged("Layer");
            }
        }

        /// <summary>
        /// The map position and transform 
        /// </summary>
        [JsonProperty(Order = 6)]
        public WorldPosition Position
        {
            get { return position; }
            set 
            { 
                position = value;
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Depth index
        /// </summary>
        [JsonProperty(Order = 5)]
        public int ZIndex 
        { 
            get { return zIndex; }
            set
            {
                zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Item constructor
        /// </summary>
        /// <param name="layer">The item layer id</param>
        /// <param name="name">The item name</param>
        /// <param name="textureName">The name of the texture this item uses</param>
        public TextureItem(string layer, string name, string textureName)
        {
            // Set class type
            this.type = typeof(TextureItem).Name;

            this.layer = layer;
            this.name = name;
            this.texture = null;
            this._textureName = textureName;
            this.position = new WorldPosition();
            this.zIndex = 0;

            this.isSelected = false;
            this.isVisible = true;

            this.properties = new ObservableCollection<ItemProperty>();
        }

        /// <summary>
        /// Item constructor
        /// </summary>
        /// <param name="layer">The item layer id</param>
        /// <param name="name">The item name</param>
        /// <param name="texture">Item texture</param>
        /// <param name="position">Item world position</param>
        /// <param name="zIndex">Item depth index</param>
        public TextureItem(string layer, string name, Texture texture, WorldPosition position, int zIndex)
        {
            // Set class type
            this.type = typeof(TextureItem).Name;

            this.layer = layer;
            this.name = name;
            this.texture = texture;
            this._textureName = texture.Name;
            this.position = position;
            this.zIndex = zIndex;

            this.isSelected = false;
            this.isVisible = true;

            this.properties = new ObservableCollection<ItemProperty>();
        }
        #endregion


        #region Initialization
        /// <summary>
        /// Initializes the TextureItem after loading
        /// </summary>
        /// <param name="texture">The texture used by this item</param>
        public void Initialize(Texture texture)
        {
            this.texture = texture;
        }
        #endregion


        #region Factory Methods
        /// <summary>
        /// Creates a TextureItem
        /// </summary>
        /// <param name="layer">The layer the item is on</param>
        /// <param name="name">The name of the item</param>
        /// <param name="texture">The texture it uses</param>
        /// <param name="position">The item position</param>
        /// <param name="zIndex">The Z index depth of the item</param>
        /// <returns></returns>
        public static TextureItem Create(string layer, string name, Texture texture, WorldPosition position, int zIndex = 0)
        {
            return new TextureItem(layer, name, texture, position, zIndex);
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


        #region Notify Interface
        /// <summary>
        /// Property Changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles property change updates
        /// </summary>
        /// <param name="propertyName">The name of the property that was updated</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles property change updates
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Handles changes to the IsVisible flag
        /// </summary>
        /// <param name="e"></param>
        protected void OnIsVisibleChanged(EventArgs e)
        {
            EventHandler handler = IsVisibleChanged;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler IsVisibleChanged;


        /// <summary>
        /// Handles changes to the IsSelected flag
        /// </summary>
        /// <param name="e"></param>
        protected void OnIsSelectedChanged(EventArgs e)
        {
            EventHandler handler = IsSelectedChanged;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler IsSelectedChanged;
        #endregion


        #region Helper Methods
        /// <summary>
        /// Moves the texture item by the given amount
        /// </summary>
        /// <param name="x">Amount to move on the X axis</param>
        /// <param name="y">Amount to move on the Y axis</param>
        public void Move(int x, int y)
        {
            position.Position = new System.Drawing.Point(
                x + position.Position.X, 
                y + position.Position.Y
            );

            OnPropertyChanged("Position");
        }

        /// <summary>
        /// Sets the texture item to the given position
        /// </summary>
        /// <param name="x">Position on the X axis</param>
        /// <param name="y">Position on the Y axis</param>
        public void SetPosition(int x, int y)
        {
            position.Position = new System.Drawing.Point(x, y);

            OnPropertyChanged("Position");
        }

        /// <summary>
        /// Sets the texture items scale
        /// </summary>
        /// <param name="sX">Scale width</param>
        /// <param name="sY">Scale height</param>
        public void SetScale(float sX, float sY)
        {
            if (sX <= 0f || sY <= 0f)
                return;

            position.ScaleX = sX;
            position.ScaleY = sY;

            OnPropertyChanged("Position");
            OnPropertyChanged("Width");
            OnPropertyChanged("Height");
        }

        /// <summary>
        /// Sets the texture items width scale
        /// </summary>
        /// <param name="sX">Scale width</param>
        public void SetScaleX(float sX)
        {
            if (sX <= 0f)
                return;

            position.ScaleX = sX;

            OnPropertyChanged("Position");
            OnPropertyChanged("Width");
        }

        /// <summary>
        /// Sets the texture items height scale
        /// </summary>
        /// <param name="sX">Scale height</param>
        public void SetScaleY(float sY)
        {
            if (sY <= 0f)
                return;

            position.ScaleY = sY;

            OnPropertyChanged("Position");
            OnPropertyChanged("Height");
        }
        #endregion

    }
}
